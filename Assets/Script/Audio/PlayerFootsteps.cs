using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Configurazione Passi")]
    public float tempoTraPassiCamminata = 0.5f;
    public float tempoTraPassiCorsa = 0.4f;
    public float sogliaMovimento = 0.1f;
    public float distanzaDalSuolo = 1.3f;

    [Header("Configurazione Acqua")]
    public float raggioRadarAcqua = 1.5f;
    [Tooltip("Imposta questo SOLO sul layer dell'acqua per evitare controlli inutili su tutta la scena")]
    public LayerMask layerAcqua;

    [Header("Performance")]
    [Tooltip("Ogni quanti secondi controllare acqua/terreno sotto i piedi")]
    public float checkInterval = 0.1f;
    private float checkTimer = 0f;

    [Header("Tasti")]
    public KeyCode tastoCorsa = KeyCode.LeftShift;

    [Header("Ordine delle Texture del Terrain")]
    public SuperficieType[] ordineTextureTerrain;

    private float timerPassi;
    private Vector3 posizionePrecedente;
    private bool isGrounded;
    private SuperficieType superficieAttuale;

    void Start()
    {
        timerPassi = tempoTraPassiCamminata;
        posizionePrecedente = transform.position;
    }

    void Update()
    {
        bool staPremendoTastiMovimento = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        bool staCorrendo = Input.GetKey(tastoCorsa);

        Vector3 spostamento = transform.position - posizionePrecedente;
        spostamento.y = 0;
        float velocitaReale = spostamento.magnitude / Time.deltaTime;
        posizionePrecedente = transform.position;

        // controlla acqua/terreno solo ogni checkInterval secondi, non ogni frame
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;

            bool seiDentroLacqua = ControllaPresenzaAcqua();

            if (seiDentroLacqua)
            {
                isGrounded = true;
                superficieAttuale = SuperficieType.Acqua;
            }
            else
            {
                ControllaSuoloTramiteLaser();
            }
        }

        if (!isGrounded) return;

        if (velocitaReale > sogliaMovimento && staPremendoTastiMovimento)
        {
            timerPassi -= Time.deltaTime;

            if (timerPassi <= 0f)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayFootstep(superficieAttuale);
                }

                timerPassi = staCorrendo ? tempoTraPassiCorsa : tempoTraPassiCamminata;
            }
        }
        else
        {
            timerPassi = 0.05f;
        }
    }

    private bool ControllaPresenzaAcqua()
    {
        // usa LayerMask: controlla SOLO i collider del layer Acqua, non tutta la scena
        Collider[] collidersVicini = Physics.OverlapSphere(transform.position, raggioRadarAcqua, layerAcqua);
        return collidersVicini.Length > 0;
    }

    private void ControllaSuoloTramiteLaser()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanzaDalSuolo))
        {
            isGrounded = true;

            Terrain terrainColpito = hit.collider.GetComponent<Terrain>();
            if (terrainColpito != null)
            {
                superficieAttuale = GetTerrenoSottoIPiedi(terrainColpito, hit.point);
            }
            else
            {
                string tagOggetto = hit.collider.tag;

                if (tagOggetto == "Sabbia") superficieAttuale = SuperficieType.Sabbia;
                else if (tagOggetto == "Roccia") superficieAttuale = SuperficieType.Roccia;
                else if (tagOggetto == "Ponte") superficieAttuale = SuperficieType.Ponte;
                else superficieAttuale = SuperficieType.Erba;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    private SuperficieType GetTerrenoSottoIPiedi(Terrain terrain, Vector3 puntoImpatto)
    {
        if (terrain == null || terrain.terrainData == null) return SuperficieType.Erba;

        Vector3 terrainPos = puntoImpatto - terrain.transform.position;
        int mapX = Mathf.FloorToInt((terrainPos.x / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth);
        int mapZ = Mathf.FloorToInt((terrainPos.z / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight);
        mapX = Mathf.Clamp(mapX, 0, terrain.terrainData.alphamapWidth - 1);
        mapZ = Mathf.Clamp(mapZ, 0, terrain.terrainData.alphamapHeight - 1);

        float[,,] alphaMapData = terrain.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        int indexTextureDominante = 0;
        float pesoMaggiore = 0f;

        for (int i = 0; i < alphaMapData.GetLength(2); i++)
        {
            if (alphaMapData[0, 0, i] > pesoMaggiore)
            {
                pesoMaggiore = alphaMapData[0, 0, i];
                indexTextureDominante = i;
            }
        }

        if (ordineTextureTerrain == null || indexTextureDominante >= ordineTextureTerrain.Length) return SuperficieType.Erba;
        return ordineTextureTerrain[indexTextureDominante];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raggioRadarAcqua);
    }
}