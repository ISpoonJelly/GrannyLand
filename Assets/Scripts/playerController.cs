using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour {

    public Text txtSit;
    public GameObject txtHolder;
    private Animator anim;
    private bool sitting = false, inSit1 = false, inSit2 = false, isAniming = false;

	void Start () {
		anim = this.transform.GetChild(0).GetComponent<Animator> ();
	}

    void Update() {
        bool walking = !sitting && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S));
        bool rotating = !sitting && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));

        txtHolder.SetActive(inSit1 || inSit2);

        if (!isAniming) { 
            handleMovementAnim(walking, rotating);

            doWalking(walking);
            doRotation(rotating);
        }

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
        if (Input.GetKeyDown(KeyCode.E) && !isAniming) {
            if (sitting) {
                isAniming = true;
                sitting = false;
                txtSit.text = "Sit";
                anim.SetTrigger("sit_to_stand");
                StartCoroutine(MoveOverSeconds(this.gameObject, new Vector3(-0.02f, 0, 0.36f), 1f, false));
                StartCoroutine(finishAniming());
                return;
            }
            
            if (inSit1 || inSit2) {
                isAniming = true;
                sitting = true;
                txtSit.text = "Stand";
                anim.SetTrigger("stand_to_sit");
                this.GetComponent<Rigidbody>().isKinematic = true;
                if (inSit1) {
                    transform.rotation = Quaternion.AngleAxis(160, Vector3.up);
                    StartCoroutine(MoveOverSeconds(this.gameObject, new Vector3(0.02f, 0, -0.36f), 1f, true));
                } else if(inSit2) {
                    transform.rotation = Quaternion.AngleAxis(-45, Vector3.up);
                    StartCoroutine(MoveOverSeconds(this.gameObject, new Vector3(0.03f, 0, -0.6f), 1f, true));
                }
                StartCoroutine(finishAniming());
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

    public IEnumerator finishAniming() {
        yield return new WaitForSeconds(2.5f);
        isAniming = false;
    }


    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("sit1")) {
            inSit1 = true;
        }

        if (other.gameObject.CompareTag("sit2")) {
            inSit2 = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("sit1")) {
            inSit1 = false;
        }

        if (other.gameObject.CompareTag("sit2")) {
            inSit2 = false;
        }
    }
}