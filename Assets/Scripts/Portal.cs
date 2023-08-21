using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth
{
    public class Portal : MonoBehaviour
    {
        private GameObject portal;
        // 4 portals in total for different dimensions
        [SerializeField] private GameObject[] otherPortals = new GameObject[4];



        private void Start()
        {
            portal = gameObject;

            // force disable mesh renderer of portal
            portal.GetComponent<MeshRenderer>().enabled = false;
        }


        private void OnTriggerEnter(Collider other)
        {
            short currentDimension = other.GetComponent<Player>().Dimension;
            // check if player is in any dimension
            if (currentDimension >= 0)
            {
                // calculate height of player
                float height = other.GetComponent<Collider>().bounds.size.y;

                // change only x and z coordinates
                Vector3 tmp = otherPortals[currentDimension].transform.position;
                tmp.y += height - 0.2f; // Note (Denis): 0.2f is a small offset 
                                        // to avoid jump after the portal
                other.transform.position = tmp;

                // change rotation of player to match rotation of portal he is 
                // teleporting to, but only on y axis
                other.transform.rotation = Quaternion.Euler(0,
                otherPortals[currentDimension].transform
                .rotation.eulerAngles.y, 0);

                // rotate player by 180 on y axis (bcoz portals facing in the 
                // direction of theit input)
                other.transform.Rotate(0, 180, 0);

                // player should exit trigger to avoid teleporting back and 
                // forth
                DisablePortal(currentDimension);

                // wait until player is out of trigger
                while (other.GetComponent<Collider>().bounds.Intersects(
                otherPortals[currentDimension].GetComponent<Collider>().bounds))
                {
                    // do nothing
                }

                // wait a bit before enabling portal to avoid teleporting back 
                // and forth, do not use Inovke() because it is not working
                // properly (also coroutine is just better ^-^ )
                StartCoroutine(EnablePortalAfterDelay(currentDimension));
            }
        }


        private void DisablePortal(int index)
        {
            otherPortals[index].GetComponent<Collider>().enabled = false;
        }
        private void EnablePortal(int index)
        {
            otherPortals[index].GetComponent<Collider>().enabled = true;
        }

        private IEnumerator<WaitForSeconds> EnablePortalAfterDelay(int index)
        {
            yield return new WaitForSeconds(1);
            EnablePortal(index);
        }

        // if pressed 'ctrl+shift+alt+T' switcj mesh renderer of portals for
        // debug purposes
        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) &&
                Input.GetKey(KeyCode.LeftShift) &&
                Input.GetKey(KeyCode.LeftAlt) &&
                Input.GetKeyDown(KeyCode.T))
            {
                portal.GetComponent<MeshRenderer>().enabled =
                !portal.GetComponent<MeshRenderer>().enabled;
            }
        }

    }
}