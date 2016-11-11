using UnityEngine;

public class followPlayerBehavior : MonoBehaviour {

    //Target to Follow
	public Transform target;

    //Offset Transform between this and target
	private Vector3 offsetPosition;

	private void Start() {
		offsetPosition = this.transform.position - target.position;
	}

	private void Update() {
        transform.position = target.TransformPoint(offsetPosition);
        transform.LookAt(target);
        this.transform.Rotate(Vector3.right * -20.178f);
    }
}