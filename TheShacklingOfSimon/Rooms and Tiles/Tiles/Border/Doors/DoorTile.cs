#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Border.Doors
{
    public sealed class DoorTile : Tile
    {
        private readonly DoorTextureSet textures;
        private readonly IDoorUnlockCondition unlockCondition;

        private IRoomNavigator roomNavigator;
        private IDoorContext roomContext;

        // override these directly because doors are one of the few tiles whose blocking
        // behavior changes at runtime depending on lock state.
        public override bool BlocksGround => IsLocked;
        public override bool BlocksFly => IsLocked;
        public override bool BlocksProjectiles => true;

        public string ToRoom { get; }
        public Point SpawnGrid { get; }
        public DoorSide Side { get; }

        public bool IsLocked
        {
            get
            {
                if (roomContext == null || unlockCondition == null)
                {
                    return true;
                }

                return !unlockCondition.IsSatisfied(roomContext);
            }
        }

        public DoorTile(
            ISprite sprite,
            Vector2 position,
            string toRoom,
            Point spawnGrid,
            DoorSide side,
            DoorTextureSet textures,
            IDoorUnlockCondition unlockCondition)
            : base(sprite, position)
        {
            ToRoom = toRoom ?? "";
            SpawnGrid = spawnGrid;
            Side = side;
            this.textures = textures;
            this.unlockCondition = unlockCondition;
        }

        public void BindNavigator(IRoomNavigator navigator)
        {
            roomNavigator = navigator;
        }

        public void BindRoomContext(IDoorContext context)
        {
            roomContext = context;
        }

        public override void Update(GameTime delta)
        {
            // intentionally do nothing here.
            // Door lock state is derived from the room context instead of stored manually.
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = IsLocked ? textures.LockedUpTexture : textures.UnlockedUpTexture;

            Vector2 center = new Vector2(
                Hitbox.X + Hitbox.Width * 0.5f,
                Hitbox.Y + Hitbox.Height * 0.5f);

            Vector2 origin = new Vector2(
                texture.Width * 0.5f,
                texture.Height * 0.5f);

            float uniformScale = (RoomConstants.TileSize  *1.38f) / (float)Math.Max(texture.Width, texture.Height);

            spriteBatch.Draw(
                texture,
                center,
                null,
                Color.White,
                GetRotation(),
                origin,
                uniformScale,
                SpriteEffects.None,
                0f);
        }

        public override void OnCollision(IPlayer player)
        {
            if (player == null || roomNavigator == null)
            {
                return;
            }

            if (IsLocked)
            {
                Vector2 mtv = CollisionDetector.CalculateMinimumTranslationVector(player.Hitbox, this.Hitbox);
                if (mtv.LengthSquared() < 0.0001f) return;

                // Handles position, velocity, and hitbox
                player.SetPosition(player.Position + mtv);
                return;
            }

            //Debug.WriteLine(
            //    $"TOUCH DOOR side={Side} -> {ToRoom} spawn={SpawnGrid} locked={IsLocked}");
            roomNavigator.RequestRoomSwitch(ToRoom, SpawnGrid, player);
        }

        public override void OnCollision(IEnemy enemy)
        {
            if (enemy == null || !IsActive) return;
            ResolveEntityCollision(enemy);
        }

        public override void OnCollision(IProjectile proj)
        {
            if (proj == null || !IsActive) return;
            proj.Discontinue();
        }

        private float GetRotation()
        {
            return Side switch
            {
                DoorSide.North => 0f,
                DoorSide.East => MathHelper.PiOver2,
                DoorSide.South => MathHelper.Pi,
                DoorSide.West => -MathHelper.PiOver2,
                _ => 0f
            };
        }
    }
}