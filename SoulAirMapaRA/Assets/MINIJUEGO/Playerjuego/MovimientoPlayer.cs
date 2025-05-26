using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class MovimientoPlayer : MonoBehaviour
{
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    public Transform carrilesParent;
    [SerializeField] private Transform[] carrilesPosition;

    public Transform carrilActual;
    public int carrilActualIndex = 1; //Carril central es 1, el izquierdo es 0 y el derecho es 2
    public float speedMovement = 10f;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        //Encontrar y asignar los carriles
        carrilesPosition = new Transform[carrilesParent.childCount];
        for (int i = 0; i < carrilesParent.childCount; i++)
        {
            carrilesPosition[i] = carrilesParent.GetChild(i);
        }

        carrilActual = carrilesPosition[carrilActualIndex];
    }
    void Update()
    {
        //Detectar si se toca la pantalla
        if (Input.GetMouseButtonDown(0))
        {
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y); //Se guarda la informacion de la posicion del toque
        }
        //Detectar si se deja de tocar la pantalla
        if (Input.GetMouseButtonUp(0))
        {
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y); //Se guarda la informacion de la posicion del toque

            //Cambiar de carril
            if (currentSwipe.x < -120f) //Si el swipe es hacia la izquierda
            {
                if (carrilActualIndex > 0) //Si está en el indice 0, significa que está en el borde izquierdo
                {
                    carrilActualIndex--;
                    anim.SetBool("SwipeLeft", true); // Activar animación de swipe a la izquierda
                }
            }

            if (currentSwipe.x > 120f) //Si el swipe es hacia la derecha
            {
                if (carrilActualIndex < carrilesPosition.Length - 1) //Esto evita que se salga del borde derecho
                {
                    carrilActualIndex++;
                    anim.SetBool("SwipeRight", true); // Activar animación de swipe a la derecha
                }
            }

        }

        carrilActual = carrilesPosition[carrilActualIndex];
        transform.position = UnityEngine.Vector3.MoveTowards(transform.position, carrilActual.position, speedMovement * Time.deltaTime);
    }

    public void desactivarAnimaciones()
    {
        anim.SetBool("SwipeLeft", false);
        anim.SetBool("SwipeRight", false);
    }
}