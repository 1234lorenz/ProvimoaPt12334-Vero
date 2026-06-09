using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Configurazione Passi")]
    public float tempoTraPassiCamminata = 0.5f;
    public float tempoTraPassiCorsa = 0.3f;
    public float sogliaMovimento = 0.1f;
    public float distanzaDalSuolo = 1.3f;

    [Header("Configurazione Acqua")]
    public float raggioRadarAcqua = 1.5f;

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

        // Controlliamo l'acqua
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
        Collider[] collidersVicini = Physics.OverlapSphere(transform.position, raggioRadarAcqua);

        // Se il radar trova oggetti ma nessuno è l'acqua, questo ci aiuterà a capire
        foreach (var col in collidersVicini)
        {
            if (col.CompareTag("Acqua"))
            {
                // MESSAGGIO IN CONSOLE: Se leggi questo, il radar funziona!
                Debug.Log("<color=cyan><b>[RADAR] Acqua rilevata correttamente!</b></color>");
                return true;
            }
        }
        return false;
    }

    private void ControllaSuoloTramiteLaser()
    {
        RaycastHit hit;
        // Spariamo il laser verso il basso
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanzaDalSuolo))
        {
            isGrounded = true;

            // Se colpiamo il Terrain (la mappa di Unity)
            Terrain terrainColpito = hit.collider.GetComponent<Terrain>();
            if (terrainColpito != null)
            {
                superficieAttuale = GetTerrenoSottoIPiedi(terrainColpito, hit.point);
            }
            // SE INVECE COLPIAMO UN OGGETTO 3D (Come i Ponti!)
            else
            {
                string tagOggetto = hit.collider.tag;

                if (tagOggetto == "Sabbia") superficieAttuale = SuperficieType.Sabbia;
                else if (tagOggetto == "Roccia") superficieAttuale = SuperficieType.Roccia;
                else if (tagOggetto == "Ponte") superficieAttuale = SuperficieType.Ponte; 
                else superficieAttuale = SuperficieType.Erba; // Suono di riserva se non ha tag
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