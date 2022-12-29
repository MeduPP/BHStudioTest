using Mirror;
using Mirror.SimpleWeb;
using System.Collections;
using System.Data.Common;
using UnityEngine;


[RequireComponent(typeof(PlayerMovement))]
public class Player : NetworkBehaviour
{
    [SerializeField] private Color _injureColor;
    [SerializeField] private Renderer _colorTarget;
    [SerializeField] private float _injureTime;

    private PlayerMovement _playerMovement;

    [SyncVar(hook = nameof(SyncInjure))]
    private bool _isInjured = false;

    [SyncVar(hook = nameof(SyncColor))]
    private Color currenColor = Color.white;
    public bool IsInjured => _isInjured;

    public void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void SyncInjure(bool oldValue, bool newValue)
    {
        _isInjured = newValue;
    }

    //if the color has changed, then change the color of the player mesh
    private void SyncColor(Color oldColor, Color newColor)
    {
        Material[] materials = _colorTarget.materials;

        foreach (Material material in materials)
        {
            material.color = newColor;
        }
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

    #region Injure logic

    private void OnCollisionEnter(Collision collision)
    {
        if (_playerMovement.DashCoroutine != null && collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out Player otherPlayer))
            {
                if (isLocalPlayer && isClient)
                    SetHit(otherPlayer);
            }

            //TODO: set point to current player
        }
    }

    private void SetHit(Player player)
    {
        if (!player.IsInjured)
        {
            player.GetHit();
            player.CmdGetHit();
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
