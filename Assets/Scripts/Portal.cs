using UnityEngine;

namespace Labyrinth
{
    public class Portal : MonoBehaviour
    {
        private GameObject portal;
        [SerializeField] private GameObject otherPortal;

        private void Start()
        {
            portal = gameObject;
        }


        private void OnTriggerEnter(Collider other)
        {
            // change only x and z coordinates
            Vector3 tmp = otherPortal.transform.position;
            tmp.y = other.transform.position.y;
            other.transform.position = tmp;

            // change rotation of player to match rotation of portal he is teleporting to, but only on y axis
            other.transform.rotation = Quaternion.Euler(0, otherPortal.transform.rotation.eulerAngles.y, 0);

            // rotate player by 180 on y axis
            other.transform.Rotate(0, 180, 0);

            // player should exit trigger to avoid teleporting back and forth
            otherPortal.GetComponent<Collider>().enabled = false;
            Invoke(nameof(EnablePortal), 1f);
        }

        private void EnablePortal()
        {
            otherPortal.GetComponent<Collider>().enabled = true;
        }

    }
}