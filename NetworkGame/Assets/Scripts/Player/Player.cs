using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public string playerName;
    public int health = 100;
    private float lastAttackTime;
    private float attackCooldown;
    private Animator animationPlayer;
    // Use this for initialization
    void Start () {
        //PlayerManager.Instance.AddNewPlayer(this);
        attackCooldown = 1.0f;
        lastAttackTime = Time.time;
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
        if(lastAttackTime + attackCooldown <= Time.time)
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
    }
}
