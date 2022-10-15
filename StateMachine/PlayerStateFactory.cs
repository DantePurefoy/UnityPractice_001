public class PlayerStateFactory
{
    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Airborne() 
    {
        return new PlayerAirborneState();
    }
    public PlayerBaseState Grounded() 
    {
        return new PlayerGroundedState();
    }
    public PlayerBaseState Idle()
    {
        return new PlayerIdleState();
    }
    public PlayerBaseState Walk() 
    {
        return new PlayerWalkState();
    }
    public PlayerBaseState Sprint()
    {
        return new PlayerSprintState();
    }
    public PlayerBaseState Jump() 
    {
        return new PlayerJumpState();
    }
    public PlayerBaseState Fall() 
    {
        return new PlayerFallState();
    }
    public PlayerBaseState WallRun() 
    {
        return new PlayerWallRunState();
    }
    public PlayerBaseState WallClimb() 
    {
        return new PlayerWallClimbState();
    }
    public PlayerBaseState LedgeGrab()
    {
        return new PlayerLedgeGrabState();
    }
    public PlayerBaseState Vault() 
    {
        return new PlayerVaultState();
    }
}
