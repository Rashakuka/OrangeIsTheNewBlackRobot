using System;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using Robocode.TankRoyale.BotApi.Graphics;

// ------------------------------------------------------------------
// OrangeIsTheNewBlack.Support4
// ------------------------------------------------------------------
// Support 4 bot - Roaming support and ganker
// High mobility, supports team, roams for kills
// ------------------------------------------------------------------
public class OrangeIsTheNewBlack_Support4 : Bot
{
    private double lastEnemyDistance;
    private int roamDirection;
    private bool isRoaming;
    private int supportMode;
    private double moveAmount;

    // The main method starts our bot
    static void Main()
    {
        new OrangeIsTheNewBlack_Support4().Start();
    }

    // Called when a new round is started -> initialize and do some movement
    public override void Run()
    {
        // Set colors - Blue for support
        BodyColor = Color.Blue;
        TurretColor = Color.Blue;
        RadarColor = Color.Cyan;
        BulletColor = Color.Yellow;
        ScanColor = Color.Yellow;

        // Initialize variables
        roamDirection = 1;
        isRoaming = true;
        supportMode = 0;
        moveAmount = Math.Max(ArenaWidth, ArenaHeight) / 3;
        lastEnemyDistance = 0;

        // Start with roaming behavior
        TurnGunLeft(90);
        Forward(moveAmount);
        
        // Main loop - roaming support behavior
        while (IsRunning)
        {
            // Roaming support movement pattern
            switch (supportMode)
            {
                case 0: // Fast roaming
                    TurnGunRight(60);
                    Forward(60);
                    break;
                case 1: // Scanning while roaming
                    TurnGunLeft(120);
                    TurnLeft(30);
                    break;
                case 2: // Support positioning
                    TurnGunRight(90);
                    Forward(40);
                    break;
            }
            
            // Cycle through support modes
            supportMode = (supportMode + 1) % 3;
            
            // Change roam direction for unpredictable movement
            if (supportMode == 0)
            {
                roamDirection *= -1;
                TurnLeft(roamDirection * 45);
            }
        }
    }

    // We scanned another bot -> support response
    public override void OnScannedBot(ScannedBotEvent e)
    {
        lastEnemyDistance = e.Distance;
        
        // Support 4 behavior - aggressive support
        if (e.Distance < 250)
        {
            // Close support - engage
            TurnLeft(e.Bearing);
            Forward(40);
            
            // Support fire - moderate damage
            SetFire(2);
            TurnGunLeft(e.Bearing);
        }
        else if (e.Distance < 400)
        {
            // Medium range support - position for gank
            TurnLeft(e.Bearing + 30);
            Forward(30);
            
            // Support fire
            SetFire(1);
            TurnGunLeft(e.Bearing);
        }
        else
        {
            // Long range support - roam to position
            TurnLeft(e.Bearing);
            Forward(50);
        }
        
        // Rescan for continuous support
        Rescan();
    }

    // We hit another bot -> support response
    public override void OnHitBot(HitBotEvent e)
    {
        // Support 4 - aggressive support
        SetFire(2);
        TurnGunLeft(e.Bearing);
        
        // Support positioning - stay close but mobile
        if (e.Distance < 150)
        {
            // Close support - circle around
            TurnLeft(e.Bearing + 90);
            Forward(30);
        }
        else
        {
            // Medium support - maintain distance
            TurnLeft(e.Bearing);
            Forward(25);
        }
    }

    // We got hit -> support mobility
    public override void OnHitByBullet(HitByBulletEvent e)
    {
        // Support mobility - dodge and reposition
        TurnGunLeft(e.Bearing);
        SetFire(1);
        
        // Quick repositioning
        TurnLeft(90);
        Forward(50);
        
        // Change roam direction
        roamDirection *= -1;
    }

    // We hit a wall -> support repositioning
    public override void OnHitWall(HitWallEvent e)
    {
        // Support repositioning - quick and mobile
        TurnLeft(90);
        Forward(60);
        
        // Change roam pattern
        roamDirection *= -1;
    }

    // We won -> support celebration
    public override void OnWonRound(WonRoundEvent e)
    {
        // Support victory dance - quick and mobile
        for (int i = 0; i < 8; i++)
        {
            TurnLeft(45);
            Forward(15);
        }
    }
}