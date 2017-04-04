using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Wartorn.ScreenManager;
using Wartorn.Storage;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.Utility;


namespace Wartorn.Drawing
{
    class Camera
    {
        private float _zoom = 1f;
        private Vector2 _location = Vector2.Zero;
        public float _rotation = 0f;

        /// <summary>
        /// Zoom ratio. default value is 1f
        /// </summary>
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
            }
        }

        /// <summary>
        /// Location of the camera. default value is Vector2.Zero
        /// </summary>
        public Vector2 Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }

        /// <summary>
        /// Rotation of the camera in radiant. default value is 0f
        /// </summary>
        public float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }
        
        private Rectangle _bounds { get; set; }

        public Matrix TransformMatrix
        {
            get
            {
                return
                    Matrix.CreateTranslation(new Vector3(-_location.X, -_location.Y, 0)) *
                    Matrix.CreateRotationZ(_rotation) *
                    Matrix.CreateScale(_zoom);// *
                    //Matrix.CreateTranslation(new Vector3(_bounds.Width * 0.5f, _bounds.Height * 0.5f, 0));
            }
        }

        public Camera(Viewport viewport)
        {
            _bounds = viewport.Bounds;
        }

        public Vector2 TranslateFromScreenToWorld(Vector2 vt)
        {
            return Vector2.Transform(vt, Matrix.Invert(this.TransformMatrix));
        }
    }
}