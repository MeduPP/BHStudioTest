using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerMovement))]
public class Player : NetworkBehaviour
{
    [SerializeField] private Color _injureColor;
    [SerializeField] private Renderer _colorTarget;
    [SerializeField] private float _injureTime;

    private PlayerMovement _playerMovement;

    // Events that the PlayerInfoUI will subscribe to
    public event System.Action<byte> OnPlayerNumberChanged;
    public event System.Action<Color> OnPlayerColorChanged;
    public event System.Action<ushort> OnPlayerScoreChanged;

    // Players List to manage playerNumber
    static readonly List<Player> playersList = new List<Player>();

    [SerializeField] private GameObject playerInfoUIPrefab;

    GameObject playerInfoUIObject;
    PlayerUI playerInfoUI = null;
    public bool IsInjured => _isInjured;

    #region SyncVars
    [SyncVar(hook = nameof(InjureStateChanged))]
    private bool _isInjured = false;

    //plaer's mesh collor
    [SyncVar(hook = nameof(SyncColor))]
    private Color currenColor = Color.white;

    [SyncVar(hook = nameof(PlayerTextColorChanged))]
    public Color playerTextColor = Color.white;

    [SyncVar(hook = nameof(PlayerNumberChanged))]
    public byte playerNumber = 0;

    [SyncVar(hook = nameof(PlayerScoreChanged))]
    public ushort playerScore = 0;


    void PlayerNumberChanged(byte _, byte newPlayerNumber)
    {
        OnPlayerNumberChanged?.Invoke(newPlayerNumber);
    }

    void PlayerTextColorChanged(Color _, Color newPlayerColor)
    {
        OnPlayerColorChanged?.Invoke(newPlayerColor);
    }

    void PlayerScoreChanged(ushort _, ushort newPlayerScore)
    {
        OnPlayerScoreChanged?.Invoke(newPlayerScore);
    }

    private void InjureStateChanged(bool _, bool newValue)
    {
        _isInjured = newValue;
    }

    //if the color has changed, then change the color of the player mesh
    private void SyncColor(Color _, Color newColor)
    {
        Material[] materials = _colorTarget.materials;

        foreach (Material material in materials)
        {
            material.color = newColor;
        }
    }
    #endregion

    public void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    //if the player hits an obstacle while dashing, cancel the dash
    private void OnCollisionStay(Collision collision)
    {
        if (_playerMovement.DashCoroutine != null && collision.gameObject.CompareTag("Obstacle"))
        {
            if (isLocalPlayer && isClient)
                _playerMovement.StopDash();
        }
    }

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Add this to the static Players List
        playersList.Add(this);

        // set the Player Color SyncVar
        playerTextColor = Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);

        // set the initial player score
        playerScore = 0;

    }

    [ServerCallback]
    internal static void ResetPlayerNumbers()
    {
        byte playerNumber = 1;
        foreach (Player player in playersList)
            player.playerNumber = playerNumber++;
    }

    public override void OnStopServer()
    {
        CancelInvoke();
        playersList.Remove(this);
    }
    #endregion

    #region Client
    public override void OnStartClient()
    {
        // Instantiate the player UI as child of the Players Panel
        playerInfoUIObject = Instantiate(playerInfoUIPrefab, CanvasUI.GetPlayersPanel());
        playerInfoUI = playerInfoUIObject.GetComponent<PlayerUI>();

        // wire up all events to handlers in PlayerUI
        OnPlayerNumberChanged = playerInfoUI.OnPlayerNumberChanged;
        OnPlayerColorChanged = playerInfoUI.OnPlayerColorChanged;
        OnPlayerScoreChanged = playerInfoUI.OnPlayerScoreChanged;

        // Invoke all event handlers with the initial data from spawn payload
        OnPlayerNumberChanged.Invoke(playerNumber);
        OnPlayerColorChanged.Invoke(playerTextColor);
        OnPlayerScoreChanged.Invoke(playerScore);
    }

    public override void OnStopClient()
    {
        // disconnect event handlers
        OnPlayerNumberChanged = null;
        OnPlayerColorChanged = null;
        OnPlayerScoreChanged = null;

        // Remove this player's UI object
        Destroy(playerInfoUIObject);
    }

    #endregion

    #region Injure logic

    private void OnCollisionEnter(Collision collision)
    {
        if (_playerMovement.DashCoroutine != null && collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out Player otherPlayer))
            {
                if (isLocalPlayer && isClient)
                {
                    SetHit(otherPlayer);
                }
            }
        }
    }

    [Command]
    private void CmdAddScore()
    {
        playerScore++;
    }

    private void SetHit(Player player)
    {
        if (!player.IsInjured)
        {
            player.GetHit();
            player.CmdGetHit();
            CmdAddScore();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdGetHit()
    {
        _isInjured = true;
        StartCoroutine(InInjure());
    }

    public void GetHit()
    {
        _isInjured = true;
        if (isClient && isLocalPlayer)
            StartCoroutine(InInjure());
    }

    IEnumerator InInjure()
    {
        currenColor = _injureColor;
        yield return new WaitForSeconds(_injureTime);
        currenColor = Color.white;

        _isInjured = false;
    }
    #endregion
}
