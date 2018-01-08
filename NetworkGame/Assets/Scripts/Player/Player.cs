using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public string playerName;
    public int health = 100;
    private float lastAttackTime;
    private float attackCooldown;
    private float attackCooldownBanded;
    private Animator animationPlayer;
    public bool IsAlive { get; set; }
    private float respawnTime = 5.0f;
    // Use this for initialization
    void Start () {
        //PlayerManager.Instance.AddNewPlayer(this);
        attackCooldown = 1.0f;
        attackCooldownBanded = 1.5f;
        lastAttackTime = Time.time;
        IsAlive = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void SetPosition(string x, string y, string z)
    {
        transform.position = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }

    public string GetPositionString()
    {
        return playerName + " pos " + transform.position.x + " " + transform.position.y + " " + transform.position.z;
    }

    public string GetRotationString()
    {
        Vector3 rotvec = transform.rotation.eulerAngles;
        return playerName + " rot " + rotvec.x + " " + rotvec.y + " " + rotvec.z;
    }

    public virtual void SetRotation(string x, string y, string z)
    {
        transform.rotation = Quaternion.Euler(float.Parse(x), float.Parse(y), float.Parse(z));
    }

    public string GetHealthString()
    {
        return "hl " + playerName + " " + health;
    }

    public string GetAliveString()
    {
        return "al " + playerName + " " + IsAlive.ToString();
    }

    public void OnTriggerEnter(Collider other)
    {   
        if(other.gameObject.tag == "Weapon")
        {
            if (other.gameObject.transform.parent == this.gameObject)
            {
                Debug.Log("Colliding with own sword");
            } else
            {
                PlayerManager.Instance.DamagePlayer(playerName, 10);
                Debug.Log("Colliding with other player's sword");
                if (TestNetworkScript.Instance.IsServer)
                {
                    TestNetworkScript.Instance.SendNetworkMessageToAllClients("servermsg " + GetHealthString(), true);
                }
            }
        }
    }

    public bool TryAttack()
    {
        float cd = (TestNetworkScript.Instance.IsServer ? attackCooldown : attackCooldownBanded);
        if(lastAttackTime + cd <= Time.time)
        {
            if (animationPlayer == null)
            {
                animationPlayer = GetComponentInChildren<Animator>();
            }

            animationPlayer.Play("SwordSlash");
            lastAttackTime = Time.time;
            return true;
        }

        return false;
    }

    public void PlayAnimation(string animName)
    {
        if (animationPlayer == null)
        {
            animationPlayer = GetComponentInChildren<Animator>();
        }
        animationPlayer.Play(animName);
        if (TestNetworkScript.Instance.IsServer)
        {
            TestNetworkScript.Instance.SendNetworkMessageToServer("servermsg " + playerName + " at1", true);
        }
    }

    public void Die(bool thisPlayer)
    {
        if (thisPlayer) PlayerManager.Instance.ShowDeathText(true);
        PlayAnimation("SwordDeath");
        if (IsAlive)
        {
            IsAlive = false;
            if (TestNetworkScript.Instance.IsServer)
            {
                StartCoroutine(RespawnTimer());
                TestNetworkScript.Instance.SendNetworkMessageToAllClients("servermsg " + GetAliveString(), true);
            }
        }
    }

    public IEnumerator RespawnTimer()
    {
        float deathTime = Time.time;
        while(deathTime + respawnTime >= Time.time)
        {
            yield return null;
        }
        Respawn();
    }

    public void Respawn(bool thisPlayer = false)
    {
        health = 100;
        IsAlive = true;
        SetPosition("0", "1.0", "0");
        if(thisPlayer) PlayerManager.Instance.ShowDeathText(false);
        if (TestNetworkScript.Instance.IsServer)
        {
            TestNetworkScript.Instance.SendNetworkMessageToAllClients("servermsg " + GetHealthString(), true);
            TestNetworkScript.Instance.SendNetworkMessageToAllClients("servermsg " + GetAliveString(), true);
            TestNetworkScript.Instance.SendNetworkMessageToAllClients(GetPositionString(), true);
        }
    }
}
