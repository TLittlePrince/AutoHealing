using System;
using System.Collections;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using Version = System.Version;

namespace AutoHealing
{
    public class Plugin : Plugin<Config>
    {
        public override string Name { get; } = "AutoHealing";
        public override string Author { get; } = "Little Prince";
        public override Version Version { get; } = new Version(1, 0, 1);

        private EventHandlers EventHandlers { get; set; }

        // 注册事件
        public override void OnEnabled()
        {
            EventHandlers = new EventHandlers(Config);
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Player.ChangingMoveState += EventHandlers.OnChangingMoveState;
            Exiled.Events.Handlers.Player.Spawned += EventHandlers.OnSpawned;
            Exiled.Events.Handlers.Player.Left += EventHandlers.OnLeft;
            base.OnEnabled();
        }

        // 取消注册事件
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Player.ChangingMoveState -= EventHandlers.OnChangingMoveState;
            Exiled.Events.Handlers.Player.Spawned -= EventHandlers.OnSpawned;
            Exiled.Events.Handlers.Player.Left -= EventHandlers.OnLeft;
            EventHandlers = null;
            base.OnDisabled();
        }
    }

    public class EventHandlers
    {
        private readonly Config _config;
        private ArrayList _joinedPlayers = new ArrayList();
        private Hashtable _movementDetectors = new Hashtable();
        private readonly Hashtable _autoRecovery = new Hashtable();

        public EventHandlers(Config instance)
        {
            _config = instance;
        }

        ~EventHandlers()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Player.ChangingMoveState -= OnChangingMoveState;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            _joinedPlayers.Clear();
            _movementDetectors.Clear();
            _joinedPlayers = null;
            _movementDetectors = null;
        }

        public void OnRoundStarted()
        {
            // Map.Broadcast((ushort)5f,
            //     $"TestValue: {_config.TreatmentPerSecond}\n" +
            //     $"IsEnabled: {_config.IsEnabled}\n" +
            //     $"Debug: {_config.Debug}");
        }

        public static void OnChangingMoveState(ChangingMoveStateEventArgs ev)
        {
            // ev.Player.Broadcast(2, $"New state: {ev.NewState}", Broadcast.BroadcastFlags.Normal, true);
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            // ev.Player.Health = ev.Player.MaxHealth / 2;
            if (_autoRecovery.ContainsKey(ev.Player.Id))
            {
                try
                {
                    ((Test)_autoRecovery[ev.Player.Id]).Stop();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                _autoRecovery.Remove(ev.Player.Id);
            }
            {
                
            }
            var t = new Test(ev.Player, _config.TreatmentPerSecond);
            t.Start();
            _autoRecovery.Add(ev.Player.Id, t);
        }

        /**
         * 玩家加入事件
         */
        // public void OnSpawned(SpawnedEventArgs ev)
        // {
        //     ev.Player.Health = ev.Player.MaxHealth / 2;
        //     _joinedPlayers.Add(ev.Player);
        //     if (_movementDetectors.ContainsKey(ev.Player.Id))
        //     {
        //         try
        //         {
        //             ((PlayerMovementDetector)_movementDetectors[ev.Player.Id]).Stop();
        //         }
        //         catch (Exception e)
        //         {
        //             Console.WriteLine(e);
        //         }
        //         _movementDetectors.Remove(ev.Player.Id);
        //     }
        //     var detector = new PlayerMovementDetector(ev.Player);
        //     _movementDetectors.Add(ev.Player.Id, detector);
        //
        //     if (_autoRecovery.Contains(ev.Player.Id))
        //     {
        //         try
        //         {
        //             ((AutoRecovery)_autoRecovery[ev.Player.Id]).Stop();
        //         }
        //         catch (Exception e)
        //         {
        //             Console.WriteLine(e);
        //         }
        //         _autoRecovery.Remove(ev.Player.Id);
        //     }
        //     var autoRecovery = new AutoRecovery(ev.Player, _config.TreatmentPerSecond);
        //     _autoRecovery.Add(ev.Player.Id, autoRecovery);
        //     
        //     // var randomItemId = (ItemType)UnityEngine.Random.Range(0, 50);
        //     // ev.Player.AddItem(randomItemId);
        //     // ev.Player.Broadcast(1, $"Random item: {Enum.GetName(typeof(ItemType), randomItemId)}", Broadcast.BroadcastFlags.Normal, true);
        //     
        //     detector.PlayerMovement += (isMoving) =>
        //     {
        //         /*ev.Player.Broadcast(1, detector.IsMoving() ? "You are moving!" : "You are not moving!",
        //             Broadcast.BroadcastFlags.Normal, true);*/
        //         // ev.Player.Broadcast(1, $"{ev.Player.IsScp}", Broadcast.BroadcastFlags.Normal, true);
        //         if (!ev.Player.IsScp) return;
        //         var ar = (AutoRecovery)_autoRecovery[ev.Player.Id];
        //         if (ev.Player.IsDead || ev.Player.IsOverwatchEnabled || ev.Player.IsGodModeEnabled)
        //         {
        //             ev.Player.Broadcast(1, "You are dead or in god mode, auto recovery stopped!",
        //                 Broadcast.BroadcastFlags.Normal, true);
        //             try
        //             {
        //                 ar.Stop();
        //             }
        //             catch (Exception e)
        //             {
        //                 Console.WriteLine(e);
        //             }
        //             return;
        //         }
        //         ev.Player.Broadcast(1, $"IsRunning: {ar.IsRunning}, IsMoving: {isMoving}", Broadcast.BroadcastFlags.Normal, true);
        //         if (isMoving)
        //         {
        //             ev.Player.Broadcast(1, "You are moving, auto recovery stopped!", Broadcast.BroadcastFlags.Normal,
        //                 true);
        //             try
        //             {
        //                 ar.Stop();
        //             }
        //             catch (Exception e)
        //             {
        //                 Console.WriteLine(e);
        //             }
        //         }
        //         else if (!ar.IsRunning)
        //         {
        //             ev.Player.Broadcast(1, "You are not moving, auto recovery started!",
        //                 Broadcast.BroadcastFlags.Normal, true);
        //             ar.Start();
        //         }
        //     };
        //     if (!detector.IsRunning)
        //     {
        //         detector.Start();
        //     }
        //     /*if (_movementDetectors.ContainsKey(ev.Player.Id))
        //     {
        //         ((PlayerMovementDetector)_movementDetectors[ev.Player.Id]).Stop();
        //         _movementDetectors.Remove(ev.Player.Id);
        //     }*/
        //     if (ev.Player.IsScp && !autoRecovery.IsRunning)
        //     {
        //         autoRecovery.Start();
        //     }
        // }
        public void OnLeft(LeftEventArgs ev)
        {
            _joinedPlayers.Remove(ev.Player);
            if (_autoRecovery.ContainsKey(ev.Player.Id))
            {
                try
                {
                    ((Test)_autoRecovery[ev.Player.Id]).Stop();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                _autoRecovery.Remove(ev.Player.Id);
            }
        }
    }

    /*public delegate void PlayerMovementHandler(bool isMoving);

    public sealed class PlayerMovementDetector
    {
        private Vector3 _lastPosition;
        private readonly Player _player;
        private readonly Thread _childThread;
        private PlayerMovementHandler _playerMovementHandler;

        public event PlayerMovementHandler PlayerMovement
        {
            add => _playerMovementHandler += value;
            remove => _playerMovementHandler -= value;
        }

        public PlayerMovementDetector(Player player)
        {
            _player = player;
            _lastPosition = player.Position;
            _childThread = new Thread(ChildThread);
            IsRunning = false;
        }

        private void ChildThread()
        {
            _player.Broadcast(2, "Thread started!", Broadcast.BroadcastFlags.Normal, true);
            while (true)
            {
                CheckUpdate();
                Thread.Sleep(1000);
            }
        }

        private void CheckUpdate()
        {
            Vector3 playerPosition;
            try
            {
                playerPosition = _player.Position;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (playerPosition != _lastPosition)
            {
                _lastPosition = playerPosition;
                _playerMovementHandler.Invoke(true);
            }
            else
            {
                _playerMovementHandler.Invoke(false);
            }
        }

        public bool IsRunning { get; private set; }

        public void Start()
        {
            _childThread.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            _childThread.Abort();
            IsRunning = false;
        }
    }

    public class AutoRecovery
    {
        private readonly Player _player;
        private readonly float _healingSteps;
        private Thread _childThread;

        public AutoRecovery(Player player, float healingSteps = 1)
        {
            _player = player;
            _healingSteps = healingSteps;
        }

        private void ChildThread()
        {
            while (IsRunning)
            {
                Healing();
                _player.Broadcast(1, $"Health: {_player.Health}", Broadcast.BroadcastFlags.Normal, true);
                Thread.Sleep(2000);
            }
        }

        private void Healing()
        {
            if (_player.Health <= _player.MaxHealth) _player.Health += _healingSteps;
        }

        public void Start()
        {
            _childThread = new Thread(ChildThread);
            _childThread.Start();
            IsRunning = true;
            // _player.Broadcast(1, "AutoRecovery started!", Broadcast.BroadcastFlags.Normal, true);
        }

        public void Stop()
        {
            _childThread.Abort();
            IsRunning = false;
            // _player.Broadcast(1, "AutoRecovery stopped!", Broadcast.BroadcastFlags.Normal, true);
        }

        public bool IsRunning { get; private set; }
    }*/

    public class Test
    {
        private readonly Player _player;
        private readonly float _maxHealth;
        private readonly float _healingSteps;
        private CoroutineHandle _coroutineHandle;
        private Vector3 _lastPosition;

        public Test(Player player, float healingSteps = 1)
        {
            _player = player;
            _maxHealth = player.MaxHealth;
            _healingSteps = healingSteps;
            _lastPosition = player.Position;
        }
        
        public void Start()
        {
            _coroutineHandle = Timing.RunCoroutine(Healing());
        }
        
        public void Stop()
        {
            Timing.KillCoroutines(_coroutineHandle);
        }

        private IEnumerator<float> Healing()
        {
            for (;;)
            {
                /*Log.Info($"Player: {_player.Position.x}, {_player.Position.y}, {_player.Position.z}");
                Log.Info($"MaxHealth: {_player.MaxHealth}, Health: {_player.Health}");*/
                if (_player.Position != _lastPosition)
                {
                    // Log.Info("Player is moving!");
                }
                else
                {
                    // Log.Info("Player is not moving!");
                    if (_player.IsScp && !(_player.IsDead || _player.IsOverwatchEnabled || _player.IsGodModeEnabled))
                    {
                        if (_player.Health + _healingSteps > _maxHealth)
                        {
                            _player.Health = _maxHealth;
                        }
                        else
                        {
                            _player.Health += _healingSteps;
                        }
                    }
                }
                _lastPosition = _player.Position;
                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}