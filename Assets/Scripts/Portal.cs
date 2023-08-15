using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth
{
    public class Portal : MonoBehaviour
    {
        /// <summary>
        /// Portal script
        /// </summary>
        /// <remarks>
        /// This script is responsible for teleporting player to another
        /// portal
        /// 
        /// ASSUMING:
        /// - tag of player is "Player"
        /// </remarks>

        private GameObject portal; // portal itself, assigned in Start()

        // 4 other portals in total for different dimensions
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
                tmp.y += height /*- 0.2f*/; // Note (Denis): 0.2f is a small 
                                            // offset to avoid jump after the 
                                            // portal (if happens)
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
                DisablePortalCollider(currentDimension);

                // wait until player is out of trigger
                while (other.GetComponent<Collider>().bounds.Intersects(
                    otherPortals[currentDimension].GetComponent<Collider>()
                    .bounds))
                {
                    // do nothing
                }

                // wait a bit before enabling portal to avoid teleporting back 
                // and forth, do not use Inovke() because it is not working
                // properly (also coroutine is just better ^-^ )
                StartCoroutine(EnablePortalAfterDelay(currentDimension));
            }
        }


        private void DisablePortalCollider(int index)
        {
            /// <summary>
            /// Disable portal's collider to avoid teleporting back and forth
            /// </summary>
            /// <param name="index">
            /// Index of portal to disable
            /// </param>
            otherPortals[index].GetComponent<Collider>().enabled = false;
        }
        private void EnablePortalCollider(int index)
        {
            /// <summary>
            /// Enable portal's collider to allow teleporting
            /// </summary>
            /// <param name="index">
            /// Index of portal to enable
            /// </param>
            otherPortals[index].GetComponent<Collider>().enabled = true;
        }

        private IEnumerator<WaitForSeconds> EnablePortalAfterDelay(int index)
        {
            /// <summary>
            /// Enable portal after 1 second to avoid teleporting back and
            /// forth
            /// </summary>
            /// <param name="index">
            /// Index of portal to enable
            /// </param>
            yield return new WaitForSeconds(1);
            EnablePortalCollider(index);
        }

        private int UpdatePortalColor()
        {
            /// <summary>
            /// Change portal's albedo color to match dimension
            /// 0 = #00FF0080
            /// 1 = #0000FF80
            /// 2 = #FF00FF80
            /// 3 = #FFFF0080
            /// </summary>
            /// <returns>
            /// 1 if success
            /// 0 if fail
            /// </returns>
            try
            {
                GameObject other = GameObject.FindGameObjectWithTag("Player");
                switch (other.GetComponent<Player>().Dimension)
                {
                    case 0:
                        portal.GetComponent<MeshRenderer>().material.color =
                            new Color(0, 1, 0, 0.5f);
                        break;
                    case 1:
                        portal.GetComponent<MeshRenderer>().material.color =
                            new Color(0, 0, 1, 0.5f);
                        break;
                    case 2:
                        portal.GetComponent<MeshRenderer>().material.color =
                            new Color(1, 0, 1, 0.5f);
                        break;
                    case 3:
                        portal.GetComponent<MeshRenderer>().material.color =
                            new Color(1, 1, 0, 0.5f);
                        break;
                }
                return 1;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        private void UpdatePortalVisibility(bool condition)
        {
            /// <summary>
            /// Change portal's visibility (ON/OFF) based on condition update
            /// </summary>
            if (condition)
            {
                portal.GetComponent<MeshRenderer>().enabled =
                    !portal.GetComponent<MeshRenderer>().enabled;
            }
        }

        private void Update()
        {
            // if pressed 'Ctrl+Shift+Alt+T' switch visibility of portals for
            // debug purposes
            UpdatePortalVisibility(
                Input.GetKey(KeyCode.LeftControl) &&
                Input.GetKey(KeyCode.LeftShift) &&
                Input.GetKey(KeyCode.LeftAlt) &&
                Input.GetKeyDown(KeyCode.T));
            // change portal's color to match Player's dimension
            UpdatePortalColor();

        }

    }
}