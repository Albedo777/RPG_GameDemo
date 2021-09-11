using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    public class JumpBehavior : StateMachineBehaviour
    {
        [Header("Jump Range to deactivate rigid body constraints while in the air")]
        public float startJump;         //Jump Normalized Start Time to Jump
        public float finishJump;        //Jump Normalized End Time to Jump

        public float FallRay = 1;       //Ray to Check if the Terrain is the same
        public float treshold = 0.5f;

        [Header("Jump Range to activate Jump Over a Cliff Transition")]
        public float startEdge = 0.5f;
        public float finishEdge = 0.6f;
        public float GroundRay = 0.5f;

        bool justActivate = false;

        float jumpPoint;

        RaycastHit JumpRay;
        Animal animal;
        Rigidbody rb;


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();
            rb = animator.GetComponent<Rigidbody>();

            if (Physics.Raycast(animal.Pivot_Hip.GetPivot, -animal.transform.up, out JumpRay, animal.Pivot_Chest.multiplier * animal.ScaleFactor, animal.GroundLayer))
            {
                jumpPoint = JumpRay.point.y;
                animal.SetIntID(0); // IDInt=0 Means that the animal is starting a Jump.
            }

            justActivate = false;
        }



        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //This code is execute when the animal can change to fall state if there's no future ground to land on
            if (Physics.Raycast(animal.Pivot_fall, -animal.transform.up, out JumpRay, animal.Pivot_Chest.multiplier * animal.ScaleFactor * FallRay, animal.GroundLayer))
            {
                if (animal.debug)
                {
                    Debug.DrawRay(animal.Pivot_fall, -animal.transform.up * animal.Pivot_Chest.multiplier * animal.ScaleFactor * FallRay, Color.red);
                }


                if ((jumpPoint - JumpRay.point.y) <= treshold * animal.ScaleFactor)   //If if finding a lower jump point;
                {
                    animal.SetIntID(0); //Keep the INTID in 0
                }
                else
                {
                    animal.SetIntID(111); //Set INTID to 111 to activate the FALL transition
                }
            }
            else
            {
                animal.SetIntID(111); //Set INTID to 111 to activate the FALL transition
            }

            //-----------------------------------------Get jumping on a cliff -------------------------------------------------------------------------------
            Debug.DrawRay(animal.Pivot_Chest.GetPivot + (animal.transform.forward * 0.1f), -animal.transform.up * GroundRay * animal.ScaleFactor, Color.yellow);

            if (Physics.Raycast(animal.Pivot_Chest.GetPivot, -animal.transform.up, out JumpRay, GroundRay * animal.ScaleFactor, animal.GroundLayer))
            {
                if (stateInfo.normalizedTime >= startEdge && stateInfo.normalizedTime <= finishEdge)
                {
                    animal.SetIntID(110);
                }
            }


            //Modify  Jump RiggidBody constraints.. when the animal is jumping Unfreeze all movement axis ... until he reaches the ground

            if (stateInfo.normalizedTime > startJump && !animator.IsInTransition(0) && !justActivate)
            {
                if (!animal.IsInAir)
                {
                    animal.IsInAir = true;
                    justActivate = true;
                }
            }

            if (stateInfo.normalizedTime >= finishJump && !animator.GetNextAnimatorStateInfo(0).IsTag("Fall"))
            {
                if (animal.IsInAir)
                {
                    animal.IsInAir = false;
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetNextAnimatorStateInfo(0).IsTag("Locomotion")) //Make sure the constraints go back to normal when exit animator to the locomotion state
            {
                rb.constraints = Animal.Still_Constraints;
                animal.SetIntID(0); //Prevent to go to Cliff Up or Fall
            }
            jumpPoint = 0;  //Resets the Jump Point
            animal.IsInAir = false;
        }
    }
}