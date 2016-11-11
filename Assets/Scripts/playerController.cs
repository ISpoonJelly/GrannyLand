using System.Collections;
using UnityEngine;

public class playerController : MonoBehaviour {

    private Animator anim;
    private bool sitting = false;

	void Start () {
		anim = this.transform.GetChild(0).GetComponent<Animator> ();
	}

	void Update () {
		bool walking = !sitting && (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S));
		bool rotating = !sitting && (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D));

        handleMovementAnim(walking, rotating);
        
		doWalking (walking);
		doRotation (rotating);

		if (!walking && !rotating) {
            handleJump();
            handleSit();
		}
	}

    private void handleMovementAnim(bool walking, bool rotating) {
        if (walking) {
            anim.SetBool("walking", true);
            anim.SetBool("rotating", false);
        } else {
            anim.SetBool("walking", false);
        }

        if (rotating) {
            if (!walking) {
                anim.SetBool("rotating", true);
            }
        } else {
            anim.SetBool("rotating", false);
        }
    }

    private void doWalking(bool walking) {
        if (!walking) return;

		if(Input.GetKey (KeyCode.W)) {
			anim.SetBool ("forward", true);
			this.transform.Translate (Vector3.forward * 0.05f);
		} else if(Input.GetKey (KeyCode.S)) {
			anim.SetBool ("forward", false);
			this.transform.Translate (Vector3.back * 0.05f);
		}
	}

	private void doRotation(bool rotating) {
        if (!rotating) return;

		if(Input.GetKey (KeyCode.A)) {
			anim.SetBool ("right", false);
			this.transform.Rotate (Vector3.down * 1.5f);
		} else if(Input.GetKey (KeyCode.D)) {
			anim.SetBool ("right", true);
			this.transform.Rotate (Vector3.up * 1.5f);
		}
    }

    private void handleJump() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!anim.GetBool("jump")) {
                anim.SetTrigger("jump");
            }
        }
    }

    private void handleSit() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (sitting) {
                sitting = false;
                anim.SetTrigger("sit_to_stand");
                StartCoroutine(MoveOverSeconds(this.gameObject, new Vector3(-0.02f, 0, 0.36f), 1f, false));
                return;
            }

            bool inPosition1 = transform.position.x >= -5.15f && transform.position.x <= -4.85f
                       && transform.position.z <= -3.95f && transform.position.z >= -4.1f;

            bool inPosition2 = transform.position.x >= 4.1f && transform.position.x <= 4.85f
                       && transform.position.z <= -4f && transform.position.z >= -4.8f;
            if (inPosition1 || inPosition2) {
                sitting = true;
                this.GetComponent<Rigidbody>().isKinematic = true;
                if (inPosition1) {
                    transform.rotation = Quaternion.AngleAxis(160, Vector3.up);
                    StartCoroutine(MoveOverSeconds(this.gameObject, new Vector3(0.02f, 0, -0.36f), 1f, true));
                } else if(inPosition2) {
                    transform.rotation = Quaternion.AngleAxis(-45, Vector3.up);
                    StartCoroutine(MoveOverSeconds(this.gameObject, new Vector3(0.03f, 0, -0.6f), 1f, true));
                }
                anim.SetTrigger("stand_to_sit");
            }
        }
    }

    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds, bool kinematic) {
        float elapsedTime = 0;
        float numSlices = seconds / Time.deltaTime;
        Vector3 slice = end / numSlices;
        while (elapsedTime < seconds) {
            transform.Translate(slice);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.GetComponent<Rigidbody>().isKinematic = kinematic;
    }
}