using System.Collections;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{

    public float open = 100f;
    public float range = 10f;

    public GameObject Entrancedoor;
    public GameObject Backdoor;
    public GameObject Kitchendoor;
    public bool isOpening = false;

    public Camera fpsCam;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                StartCoroutine(OpenDoor1());
            }
            if (target != null)
            {
                StartCoroutine(OpenDoor2());
            }
            if (target != null)
            {
                StartCoroutine(OpenDoor3());
            }
        }
    }

    IEnumerator OpenDoor1()
    {
        isOpening = true;
        Entrancedoor.GetComponent<Animator>().Play("Door");
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(5.0f);
        Entrancedoor.GetComponent<Animator>().Play("New State");
        isOpening = false;
    }

    IEnumerator OpenDoor2()
    {
        isOpening = true;
        Backdoor.GetComponent<Animator>().Play("BackDoor");
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(5.0f);
        Backdoor.GetComponent<Animator>().Play("New State");
        isOpening = false;
    }

    IEnumerator OpenDoor3()
    {
        isOpening = true;
        Kitchendoor.GetComponent<Animator>().Play("KitchenDoor");
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(5.0f);
        Kitchendoor.GetComponent<Animator>().Play("New State");
        isOpening = false;
    }
}
