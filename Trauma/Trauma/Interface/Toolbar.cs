using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;

namespace Trauma.Interface
{
    public class Toolbar
    {
        #region Constants

        private const int ICON_OFFSET = 5;

        #endregion
        #region Members

        private Texture2D background;
        public Vector2 Position;
        public Vector2 Size
        {
            get
            {
                if (orientation == Orientation.Horizontal)
                    return new Vector2(icons.Count * iconSize.X + (icons.Count - 1) * ICON_OFFSET, iconSize.Y);
                
                return new Vector2(iconSize.X, icons.Count * iconSize.Y + (icons.Count - 1) * ICON_OFFSET);
            }
        }
        private Vector2 iconSize;
        private List<Icon> icons = new List<Icon>();
        private List<bool> iconClicked = new List<bool>(); 
        private Orientation orientation;

        #endregion

        public Toolbar(Vector2 position, Vector2 iconSize, List<Texture2D> icons, Orientation orientation)
        {
            Position = position;
            this.iconSize = iconSize;

            Vector2 offset = (orientation == Orientation.Horizontal ? new Vector2(ICON_OFFSET, 0) : new Vector2(0, ICON_OFFSET));
            foreach (Texture2D texture in icons)
            {
                this.icons.Add(new Icon(texture, position + offset, new Vector2(iconSize.X, iconSize.Y)));
                offset += (orientation == Orientation.Horizontal
                               ? new Vector2(texture.Width + ICON_OFFSET, 0)
                               : new Vector2(0, texture.Height + ICON_OFFSET));
                iconClicked.Add(false);
            }

            background = ResourceManager.GetTexture("Misc_Toolbar");
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < iconClicked.Count; i++)
                iconClicked[i] = false;
            if (Input.MouseLeftButtonTapped)
            {
                Vector2 mousePos = Input.MousePosition;
                if (InBounds(mousePos))
                    SelectIconAt(mousePos);
            }
            if (Input.KeyboardTapped(Microsoft.Xna.Framework.Input.Keys.M))
                iconClicked[0] = true;
            if (Input.KeyboardTapped(Microsoft.Xna.Framework.Input.Keys.R))
                iconClicked[1] = true;
        }

        private void SelectIconAt(Vector2 mousePos)
        {
            Vector2 normalizedPos = mousePos - Position;
            float posComponent, iconSizeComponent;
            if (orientation == Orientation.Horizontal)
            {
                posComponent = normalizedPos.X;
                iconSizeComponent = iconSize.X;
            }
            else
            {
                posComponent = normalizedPos.Y;
                iconSizeComponent = iconSize.Y;
            }
            iconClicked[(int) ((int) posComponent/iconSizeComponent)] = true;
        }

        public bool IsSelected(int index)
        {
            Debug.Assert(0 <= index && index < icons.Count, "Invalid icon index.");
            return iconClicked[index];
        }

        private bool InBounds(Vector2 pos)
        {
            return !(pos.X < this.Position.X || pos.X > this.Position.X + Size.X ||
                     pos.Y < this.Position.Y || pos.Y > this.Position.Y + Size.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Position, Color.White);
            foreach (Icon icon in icons)
            {
                icon.Draw(spriteBatch);
            }
        }
    }

    public enum Orientation
    {
        Horizontal,
        Vertical,
    }
}
