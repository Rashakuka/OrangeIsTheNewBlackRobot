using System;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using Robocode.TankRoyale.BotApi.Graphics;

// ------------------------------------------------------------------
// OrangeIsTheNewBlack.MID
// ------------------------------------------------------------------
// MID lane bot - Aggressive mid-lane fighter
// High damage output, good positioning, and aggressive playstyle
// ------------------------------------------------------------------
public class OrangeIsTheNewBlack_MID : Bot
{
    private double lastEnemyDistance;
    private double lastEnemyBearing;
    private int scanCount;
    private bool isMovingForward;

    // The main method starts our bot
    static void Main()
    {
        new OrangeIsTheNewBlack_MID().Start();
    }

    // Called when a new round is started -> initialize and do some movement
    public override void Run()
    {
        // Set colors - Red for aggressive MID
        BodyColor = Color.Red;
        TurretColor = Color.Red;
        RadarColor = Color.Yellow;
        BulletColor = Color.Orange;
        ScanColor = Color.Orange;

        // Initialize variables
        scanCount = 0;
        isMovingForward = true;
        lastEnemyDistance = 0;
        lastEnemyBearing = 0;

        // Start with aggressive positioning
        TurnGunLeft(90);
        
        // Main loop - aggressive MID behavior
        while (IsRunning)
        {
            // Aggressive scanning pattern
            if (scanCount % 3 == 0)
            {
                TurnGunRight(60);
            }
            else if (scanCount % 3 == 1)
            {
                TurnGunLeft(60);
            }
            else
            {
                TurnGunRight(30);
            }
            
            scanCount++;
            
            // Move in an aggressive pattern
            if (isMovingForward)
            {
                Forward(50);
            }
            else
            {
                Back(30);
            }
            
            // Change direction periodically
            if (scanCount % 20 == 0)
            {
                isMovingForward = !isMovingForward;
                TurnLeft(45);
            }
        }
    }

    // We scanned another bot -> aggressive fire!
    public override void OnScannedBot(ScannedBotEvent e)
    {
        lastEnemyDistance = e.Distance;
        lastEnemyBearing = e.Bearing;
        
        // Aggressive MID behavior - fire immediately
        SetFire(3);
        
        // Predict enemy movement and adjust aim
        var predictedBearing = e.Bearing + (e.Velocity * Math.Sin(e.Direction * Math.PI / 180) * 0.1);
        TurnGunLeft(predictedBearing);
        
        // Move towards enemy for aggressive positioning
        if (e.Distance > 200)
        {
            TurnLeft(e.Bearing);
            Forward(30);
        }
        else if (e.Distance < 100)
        {
            TurnLeft(e.Bearing + 180);
            Back(20);
        }
        
        // Rescan for continuous tracking
        Rescan();
    }

    // We hit another bot -> aggressive response
    public override void OnHitBot(HitBotEvent e)
    {
        // MID bot fights back aggressively
        SetFire(3);
        TurnGunLeft(e.Bearing);
        
        // Move to maintain optimal distance
        if (e.Distance < 150)
        {
            Back(40);
        }
        else
        {
            Forward(20);
        }
    }

    // We got hit -> aggressive counter-attack
    public override void OnHitByBullet(HitByBulletEvent e)
    {
        // Turn towards the attacker and fire back
        TurnGunLeft(e.Bearing);
        SetFire(3);
        
        // Move to avoid further hits
        TurnLeft(90);
        Forward(30);
    }

    // We hit a wall -> reposition aggressively
    public override void OnHitWall(HitWallEvent e)
    {
        // Turn away from wall and continue aggressive movement
        TurnLeft(90);
        Forward(50);
    }
}