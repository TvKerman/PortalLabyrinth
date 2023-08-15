using UnityEngine;

namespace Labyrinth
{
    public class Player : MonoBehaviour
    {
        // current dimension of player (0 - 3)
        [SerializeField] private short dimension = 0;

        public short Dimension
        {
            get { return dimension; }
            set { dimension = value; }
        }

        private void Start()
        {
            // TODO: set default dimension to 1, as Zuev said
            // set player to dimension 0
            dimension = 0;
        }

        // Q/E to change dimension
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                dimension--;
                if (dimension < 0)
                {
                    dimension = 3;
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                dimension++;
                if (dimension > 3)
                {
                    dimension = 0;
                }
            }
        }
    }
}