#region

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Entities.Projectiles.Implementations;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Obstacles;

#endregion

namespace TheShacklingOfSimon.UI
{
    public sealed class FogOfWar : IDisposable
    {
        private const double DarkRoomChance = 0.35;

        private const float MaxFogOpacity = 0.95f;

        private const int FogBoundsPadding = 96;
        private const int MaxShaderLights = 16;
        private const int MaxFireLights = 10;
        private const int MaxFireballLights = 5;

        private const float PlayerInnerLightRadius = 25f;
        private const float PlayerOuterLightRadius = 100f;
        private const float PlayerLightStrength = 0.84f;

        private const float FireInnerLightRadius = 10f;
        private const float FireOuterLightRadius = 75f;
        private const float FireLightStrength = 0.84f;

        private const float FireballInnerLightRadius = 16f;
        private const float FireballOuterLightRadius = 50f;
        private const float FireballLightStrength = FireLightStrength;

        private const float FireClusterDistance = 20f;

        private static readonly Vector2 PlayerFogCenterOffset = new Vector2(8f, -8f);

        private readonly IPlayer _player;
        private readonly RoomManager _roomManager;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Effect _fogEffect;

        private readonly Dictionary<string, bool> _darkRooms = new Dictionary<string, bool>();
        private readonly List<FireTile> _cachedFireTiles = new List<FireTile>();
        private readonly List<LightSource> _lights = new List<LightSource>();

        private readonly Vector4[] _shaderLightData = new Vector4[MaxShaderLights];
        private readonly Vector4[] _shaderLightStrengths = new Vector4[MaxShaderLights];

        private Texture2D _blackPixel;
        private string _cachedRoomId;

        public bool IsActive { get; set; } = true;

        private readonly struct LightSource
        {
            public Vector2 Center { get; }
            public float InnerRadius { get; }
            public float OuterRadius { get; }
            public float Strength { get; }

            public LightSource(Vector2 center, float innerRadius, float outerRadius, float strength)
            {
                Center = center;
                InnerRadius = innerRadius;
                OuterRadius = outerRadius;
                Strength = MathHelper.Clamp(strength, 0f, 1f);
            }
        }

        private readonly struct FireLightSource
        {
            public Vector2 Center { get; }
            public int Count { get; }

            public FireLightSource(Vector2 center, int count)
            {
                Center = center;
                Count = count;
            }
        }

        public FogOfWar(
            IPlayer player,
            RoomManager roomManager,
            GraphicsDevice graphicsDevice,
            Effect fogEffect)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _roomManager = roomManager ?? throw new ArgumentNullException(nameof(roomManager));
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _fogEffect = fogEffect ?? throw new ArgumentNullException(nameof(fogEffect));

            CreateBlackPixel();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;
            if (_roomManager.CurrentRoom == null) return;
            if (_roomManager.CurrentRoom.TileMap == null) return;

            RefreshRoomCacheIfNeeded();

            if (!IsCurrentRoomDark()) return;

            Rectangle fogBounds = GetFogBounds();

            BuildLightSources();
            ApplyShaderParameters(fogBounds);

            spriteBatch.End();

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                _fogEffect
            );

            spriteBatch.Draw(_blackPixel, fogBounds, Color.White);

            spriteBatch.End();

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone
            );
        }

        public void Reset()
        {
            _darkRooms.Clear();
            _cachedFireTiles.Clear();
            _lights.Clear();
            _cachedRoomId = null;
        }

        private void RefreshRoomCacheIfNeeded()
        {
            string currentRoomId = _roomManager.CurrentRoom.Id;

            if (_cachedRoomId == currentRoomId)
            {
                return;
            }

            _cachedRoomId = currentRoomId;
            _cachedFireTiles.Clear();

            foreach (ITile tile in _roomManager.CurrentRoom.TileMap.PlacedTiles)
            {
                if (tile is FireTile fireTile)
                {
                    _cachedFireTiles.Add(fireTile);
                }
            }
        }

        private bool IsCurrentRoomDark()
        {
            string roomId = _roomManager.CurrentRoom.Id;

            if (string.IsNullOrWhiteSpace(roomId))
            {
                return false;
            }


            // Fire rooms are always dark so fire can act as a meaningful light source.
            if (HasActiveFireTiles())
            {
                _darkRooms[roomId] = true;
                return true;
            }

            if (!_darkRooms.TryGetValue(roomId, out bool isDark))
            {
                isDark = GetStableRoomDarkness(roomId);
                _darkRooms[roomId] = isDark;
            }


			return isDark;
        }

        private bool HasActiveFireTiles()
        {
            foreach (FireTile fireTile in _cachedFireTiles)
            {
                if (fireTile.IsActive)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool GetStableRoomDarkness(string roomId)
        {
            unchecked
            {
                int hash = 17;

                foreach (char c in roomId)
                {
                    hash = hash * 31 + c;
                }

                int positiveHash = hash & 0x7fffffff;
                double roll = positiveHash % 1000 / 1000.0;

                return roll < DarkRoomChance;
            }
        }

        private Rectangle GetFogBounds()
        {
            Rectangle roomBounds = _roomManager.CurrentRoom.TileMap.RoomBoundsWorld;

            Rectangle screenBounds = new Rectangle(
                0,
                0,
                _graphicsDevice.Viewport.Width,
                _graphicsDevice.Viewport.Height
            );

            Rectangle paddedRoomBounds = new Rectangle(
                roomBounds.X - FogBoundsPadding,
                roomBounds.Y - FogBoundsPadding,
                roomBounds.Width + FogBoundsPadding * 2,
                roomBounds.Height + FogBoundsPadding * 2
            );

            return Rectangle.Intersect(screenBounds, paddedRoomBounds);
        }

        private void BuildLightSources()
        {
            _lights.Clear();

            _lights.Add(new LightSource(
                GetPlayerLightCenter(),
                PlayerInnerLightRadius,
                PlayerOuterLightRadius,
                PlayerLightStrength
            ));

            // Add fireballs before fire tiles so moving projectile lights are less likely
            // to be dropped if the shader light limit is reached.
            AddFireballProjectileLightSources();
            AddFireLightSources();
        }

        private Vector2 GetPlayerLightCenter()
        {
            return new Vector2(
                _player.Hitbox.X + _player.Hitbox.Width / 2f,
                _player.Hitbox.Y + _player.Hitbox.Height / 2f
            ) + PlayerFogCenterOffset;
        }

        private void AddFireballProjectileLightSources()
        {
            if (_roomManager.CurrentRoom?.Entities == null)
            {
                return;
            }

            int lightsAdded = 0;

            foreach (IEntity entity in _roomManager.CurrentRoom.Entities)
            {
                if (lightsAdded >= MaxFireballLights)
                {
                    break;
                }

                if (!IsFireballProjectileEntity(entity))
                {
                    continue;
                }

                Vector2 fireballCenter = new Vector2(
                    entity.Hitbox.X + entity.Hitbox.Width / 2f,
                    entity.Hitbox.Y + entity.Hitbox.Height / 2f
                );

                _lights.Add(new LightSource(
                    fireballCenter,
                    FireballInnerLightRadius,
                    FireballOuterLightRadius,
                    FireballLightStrength
                ));

                lightsAdded++;
            }
        }

        private static bool IsFireballProjectileEntity(IEntity entity)
        {
            if (entity is FireballProjectile)
            {
                return true;
            }

            if (entity is not IProjectile projectile)
            {
                return false;
            }

            return IsFireballProjectileObject(projectile);
        }

        private static bool IsFireballProjectileObject(object projectileObject)
        {
            if (projectileObject == null)
            {
                return false;
            }

            if (projectileObject is FireballProjectile)
            {
                return true;
            }

            Type projectileType = projectileObject.GetType();

            if (projectileType.Name.Contains("FireballProjectile"))
            {
                return true;
            }

            // Handles StatusEffectProjectileDecorator and similar wrappers.
            FieldInfo baseProjectileField = projectileType.GetField(
                "_baseProjectile",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            if (baseProjectileField?.GetValue(projectileObject) is object baseProjectile)
            {
                return IsFireballProjectileObject(baseProjectile);
            }

            return false;
        }

        private void AddFireLightSources()
        {
            List<FireLightSource> fireLights = BuildFireLightClusters();

            if (fireLights.Count == 0)
            {
                return;
            }

            Vector2 playerCenter = GetPlayerLightCenter();

            fireLights.Sort((a, b) =>
                Vector2.DistanceSquared(a.Center, playerCenter)
                    .CompareTo(Vector2.DistanceSquared(b.Center, playerCenter)));

            int lightsAdded = 0;

            foreach (FireLightSource fireLight in fireLights)
            {
                if (lightsAdded >= MaxFireLights)
                {
                    break;
                }

                _lights.Add(new LightSource(
                    fireLight.Center,
                    GetFireInnerRadius(fireLight.Count),
                    GetFireOuterRadius(fireLight.Count),
                    GetFireStrength(fireLight.Count)
                ));

                lightsAdded++;
            }
        }

        private List<FireLightSource> BuildFireLightClusters()
        {
            var activeFires = new List<FireTile>();

            foreach (FireTile fireTile in _cachedFireTiles)
            {
                if (fireTile.IsActive)
                {
                    activeFires.Add(fireTile);
                }
            }

            var clusters = new List<FireLightSource>();
            var visited = new HashSet<FireTile>();

            foreach (FireTile fireTile in activeFires)
            {
                if (visited.Contains(fireTile))
                {
                    continue;
                }

                List<FireTile> cluster = BuildFireCluster(fireTile, activeFires, visited);

                if (cluster.Count == 0)
                {
                    continue;
                }

                clusters.Add(new FireLightSource(
                    GetFireClusterCenter(cluster),
                    cluster.Count
                ));
            }

            return clusters;
        }

        private static List<FireTile> BuildFireCluster(
            FireTile start,
            List<FireTile> activeFires,
            HashSet<FireTile> visited)
        {
            var cluster = new List<FireTile>();
            var queue = new Queue<FireTile>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                FireTile current = queue.Dequeue();
                cluster.Add(current);

                foreach (FireTile other in activeFires)
                {
                    if (visited.Contains(other))
                    {
                        continue;
                    }

                    if (!AreFireTilesCloseEnough(current, other))
                    {
                        continue;
                    }

                    visited.Add(other);
                    queue.Enqueue(other);
                }
            }

            return cluster;
        }

        private static bool AreFireTilesCloseEnough(FireTile a, FireTile b)
        {
            Vector2 aCenter = GetFireCenter(a);
            Vector2 bCenter = GetFireCenter(b);

            return Vector2.DistanceSquared(aCenter, bCenter) <= FireClusterDistance * FireClusterDistance;
        }

        private static Vector2 GetFireClusterCenter(List<FireTile> cluster)
        {
            Vector2 sum = Vector2.Zero;

            foreach (FireTile fireTile in cluster)
            {
                sum += GetFireCenter(fireTile);
            }

            return sum / cluster.Count;
        }

        private static Vector2 GetFireCenter(FireTile fireTile)
        {
            return new Vector2(
                fireTile.Hitbox.X + fireTile.Hitbox.Width / 2f,
                fireTile.Hitbox.Y + fireTile.Hitbox.Height / 2f
            );
        }

        private static float GetFireInnerRadius(int clusterSize)
        {
            return clusterSize switch
            {
                1 => FireInnerLightRadius,
                2 => FireInnerLightRadius * 1.08f,
                3 => FireInnerLightRadius * 1.14f,
                _ => FireInnerLightRadius * 1.20f
            };
        }

        private static float GetFireOuterRadius(int clusterSize)
        {
            return clusterSize switch
            {
                1 => FireOuterLightRadius,
                2 => FireOuterLightRadius * 1.10f,
                3 => FireOuterLightRadius * 1.18f,
                _ => FireOuterLightRadius * 1.26f
            };
        }

        private static float GetFireStrength(int clusterSize)
        {
            return clusterSize switch
            {
                1 => FireLightStrength,
                2 => MathHelper.Clamp(FireLightStrength * 1.04f, 0f, 1f),
                3 => MathHelper.Clamp(FireLightStrength * 1.08f, 0f, 1f),
                _ => MathHelper.Clamp(FireLightStrength * 1.12f, 0f, 1f)
            };
        }

        private void ApplyShaderParameters(Rectangle fogBounds)
        {
            int lightCount = Math.Min(_lights.Count, MaxShaderLights);

            for (int i = 0; i < MaxShaderLights; i++)
            {
                if (i < lightCount)
                {
                    LightSource light = _lights[i];

                    _shaderLightData[i] = new Vector4(
                        light.Center.X,
                        light.Center.Y,
                        light.InnerRadius,
                        light.OuterRadius
                    );

                    _shaderLightStrengths[i] = new Vector4(
                        light.Strength,
                        0f,
                        0f,
                        0f
                    );
                }
                else
                {
                    _shaderLightData[i] = Vector4.Zero;
                    _shaderLightStrengths[i] = Vector4.Zero;
                }
            }

            _fogEffect.Parameters["FogBounds"]?.SetValue(new Vector4(
                fogBounds.X,
                fogBounds.Y,
                fogBounds.Width,
                fogBounds.Height
            ));

            _fogEffect.Parameters["MaxFogOpacity"]?.SetValue(MaxFogOpacity);
            _fogEffect.Parameters["LightCount"]?.SetValue(lightCount);
            _fogEffect.Parameters["LightData"]?.SetValue(_shaderLightData);
            _fogEffect.Parameters["LightStrengths"]?.SetValue(_shaderLightStrengths);
        }

        private void CreateBlackPixel()
        {
            _blackPixel = new Texture2D(_graphicsDevice, 1, 1);
            _blackPixel.SetData(new[] { Color.White });
        }

        public void Dispose()
        {
            _blackPixel?.Dispose();
            _blackPixel = null;
        }
    }
}