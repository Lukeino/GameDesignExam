using UnityEngine;

public class TeleportOnCharge : MonoBehaviour
{
    public float teleportRange = 10f; // Distanza massima del teletrasporto
    public float chargeTime = 2f; // Tempo di caricamento
    public float cooldownTime = 3f; // Tempo di cooldown
    public float playerRadius = 0.5f; // Raggio del player
    public float sphereOffset = 0.3f; // Quanto la sfera deve stare fuori dall'oggetto
    public LayerMask teleportableLayers; // Layer validi per il teletrasporto
    public Transform player; // Il FirstPersonController
    public CharacterController characterController; // Per gestire il teletrasporto
    public Transform teleportIndicator; // La sfera 3D

    private float chargeTimer = 0f; // Timer di caricamento
    private float cooldownTimer = 0f; // Timer di cooldown
    private bool isCharging = false; // Se sta caricando l'abilità
    private bool isIndicatorActive = false; // Se l'indicatore è visibile o meno

    void Update()
    {
        // Gestione del cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Inizio caricamento
        if (Input.GetMouseButtonDown(1) && cooldownTimer <= 0)
        {
            isCharging = true;
            chargeTimer = 0f;
            teleportIndicator.gameObject.SetActive(true); // Mostra l'indicatore
            isIndicatorActive = true; // Indica che l'indicatore è attivo
        }

        // Se si sta caricando, aumenta il timer
        if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            // Aggiorna la posizione della sfera di teletrasporto
            UpdateTeleportIndicator();

            // Se il tasto viene rilasciato prima del tempo di caricamento, annulla
            if (Input.GetMouseButtonUp(1))
            {
                if (chargeTimer >= chargeTime)
                {
                    TryTeleport(); // Teletrasporto eseguito
                    cooldownTimer = cooldownTime; // Attiva cooldown
                }
                isCharging = false;
                teleportIndicator.gameObject.SetActive(false); // Nasconde la sfera
                isIndicatorActive = false; // Disattiva l'indicatore
            }
        }
        else
        {
            // Se l'indicatore è visibile, controlliamo se c'è una superficie su cui teletrasportarsi
            if (isIndicatorActive)
            {
                UpdateTeleportIndicator();
            }
        }
    }

    void UpdateTeleportIndicator()
    {
        Vector3 targetPosition;
        Vector3 targetNormal;
        if (FindTeleportPosition(out targetPosition, out targetNormal))
        {
            teleportIndicator.position = targetPosition + targetNormal * sphereOffset;
        }
        else if (isIndicatorActive)
        {
            // Se non c'è una superficie valida, mantieni la sfera visibile finché non viene rilasciato il tasto
            teleportIndicator.position = player.position + player.forward * teleportRange;
        }
    }

    bool FindTeleportPosition(out Vector3 targetPosition, out Vector3 targetNormal)
    {
        Camera playerCamera = Camera.main;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, teleportRange, teleportableLayers))
        {
            targetPosition = hit.point;
            targetNormal = hit.normal;

            // Se colpisce un muro (normale quasi orizzontale), sposta indietro
            if (Vector3.Dot(hit.normal, Vector3.up) < 0.5f)
            {
                targetPosition -= hit.normal * playerRadius;
            }

            return true;
        }

        targetPosition = Vector3.zero;
        targetNormal = Vector3.up;
        return false;
    }

    void TryTeleport()
    {
        Vector3 targetPosition;
        Vector3 targetNormal;
        if (FindTeleportPosition(out targetPosition, out targetNormal))
        {
            // Disattiva e riattiva il CharacterController per evitare collisioni
            characterController.enabled = false;
            player.position = targetPosition + Vector3.up * 0.1f;
            characterController.enabled = true;
        }
    }
}






