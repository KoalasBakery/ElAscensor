using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                     CAMERA CONTROLLER
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Camara que sigue al jugador suavemente con limites de escena.
 * Cuando el jugador llega al borde del nivel la camara se detiene
 * aunque el jugador siga moviendose (igual que Sally Face).
 *
 * SETUP EN UNITY:
 *   1. Agregar este script a la Main Camera
 *   2. Asignar el Transform del Player
 *   3. Configurar los limites de la camara en el Inspector
 *   4. Los limites se pueden ver en la Scene view con Gizmos
 *
 * NOTA: Conectar con sistema de escenas para cambiar limites
 *       automaticamente al cambiar de cuarto
 * ---------------------------------------------------------------
 */

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

    [Header("Limites de la camara")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    [Header("Limites activados")]
    [SerializeField] private bool useBoundsX = true;
    [SerializeField] private bool useBoundsY = false; // En side scroller Y suele ser fijo pero pos ya no se si si sera como sally o Fran

    private Camera cam;
    private float camHalfWidth;
    private float camHalfHeight;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        // Calcular mitad del tamaño de la camara
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        // Si no se asigno el player buscarlo
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Posicion deseada siguiendo al jugador
        Vector3 targetPos = player.position + offset;

        // Aplicar limites considerando el tamaño de la camara
        if (useBoundsX)
        {
            targetPos.x = Mathf.Clamp(targetPos.x,
                minX + camHalfWidth,
                maxX - camHalfWidth);
        }

        if (useBoundsY)
        {
            targetPos.y = Mathf.Clamp(targetPos.y,
                minY + camHalfHeight,
                maxY - camHalfHeight);
        }

        // Mantener Z fijo
        targetPos.z = offset.z;

        // Mover suavemente
        transform.position = Vector3.Lerp(
            transform.position, targetPos, Time.deltaTime * smoothSpeed);
    }

    // --- CAMBIAR LIMITES EN TIEMPO DE EJECUCION --- //
    // NOTa: Llamar esto al cambiar de cuarto/escena
    public void SetBounds(float newMinX, float newMaxX, float newMinY, float newMaxY)
    {
        minX = newMinX;
        maxX = newMaxX;
        minY = newMinY;
        maxY = newMaxY;
    }

    // --- GIZMOS --- //
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Dibujar los limites en la Scene view
        Vector3 center = new Vector3(
            (minX + maxX) / 2f,
            (minY + maxY) / 2f,
            0f);

        Vector3 size = new Vector3(
            maxX - minX,
            maxY - minY,
            0f);

        Gizmos.DrawWireCube(center, size);
    }
}