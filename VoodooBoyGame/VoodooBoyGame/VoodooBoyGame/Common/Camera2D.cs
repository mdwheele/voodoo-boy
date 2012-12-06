using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using System;

namespace VoodooBoyGame
{
    public class Camera2D
    {
        Vector2 position = Vector2.Zero;
        Vector2 origPosition = Vector2.Zero;
        Vector2 targetPosition = Vector2.Zero;
        float moveRate = 2.0f;
        float rotation = 0.0f;
        float origRotation = 0.0f;
        float targetRotation = 0.0f;
        float zoom = 1.0f;
        float origZoom = 1.0f;
        float targetZoom = 1.0f;
        float zoomRate = 0.1f;
        float maxZoom = 4;
        float minZoom = 0.25f;
        float rotationRate = 0.1f;
        float transition;
        bool transitioning = false;
        float horizontalMovement = 0.0f;
        float verticalMovement = 0.0f;
        const float transitionSpeed = 0.01f;
        const float smoothingSpeed = 0.15f;
        bool positionUnset = true;
        bool zoomUnset = true;
        bool rotationUnset = true;
        Vector2 size;
        Vector2 minPosition = Vector2.Zero;
        Vector2 maxPosition = Vector2.Zero;
        Body trackingBody;

        public Camera2D(Vector2 size)
        {
            this.size = size;
        }
        
        // Get set position
        public Vector2 Position
        {
            get { return position; }
            set 
            {
                if (positionUnset)
                {
                    origPosition = value;
                    positionUnset = false;
                }

                position = value;
                targetPosition = value;
            }
        }

        public void Move(Vector2 dest)
        {
            horizontalMovement = dest.X * moveRate * zoom;
            verticalMovement = dest.Y * moveRate * zoom;
        }

        public float Rotation
        {
            get { return rotation; }
            set 
            {
                if (rotationUnset)
                {
                    origRotation = value;
                    rotationUnset = false;
                }

                rotation = value;
                targetRotation = value;
            }
        }

        // Sets and gets zoom
        public float Zoom
        {
            get { return zoom; }
            set 
            {
                if (zoomUnset)
                {
                    origZoom = value;
                    zoomUnset = false;
                }

                zoom = value;
                targetZoom = value;
            }
        }

        public void ZoomIn()
        {
            targetZoom = MathHelper.Min(maxZoom, zoom + zoomRate);
        }

        public void ZoomOut()
        {
            targetZoom = MathHelper.Max(minZoom, zoom - zoomRate);
        }

        public void RotateLeft()
        {
            targetRotation = (rotation - rotationRate) % (float)(Math.PI * 2);
        }

        public void RotateRight()
        {
            targetRotation = (rotation + rotationRate) % (float)(Math.PI * 2);
        }

        public bool ClampingEnabled
        {
            get { return this.minPosition != this.maxPosition; }
        }

        public float MaxZoom
        {
            get { return maxZoom; }
            set { maxZoom = value; }
        }

        public float MinZoom
        {
            get { return minZoom; }
            set { minZoom = value; }
        }

        public float RotationRate
        {
            get { return rotationRate; }
            set { rotationRate = value; }
        }

        public float ZoomRate
        {
            get { return zoomRate; }
            set { zoomRate = value; }
        }

        public float MoveRate
        {
            get { return moveRate; }
            set { moveRate = value; }
        }

        public Vector2 Size
        {
            get { return size; }
        }

        public Vector2 CurSize
        {
            get { return Vector2.Multiply(size, 1 / zoom); }
        }

        public Matrix CameraMatrix
        {
            get
            {
                return Matrix.Identity *
                      Matrix.CreateTranslation(new Vector3(-position, 0)) *
                      Matrix.CreateScale(zoom) *
                      Matrix.CreateRotationZ(rotation) *
                      Matrix.CreateTranslation(new Vector3(size / 2, 0));
            }
        }

        public Vector2 MinPosition
        {
            get { return minPosition; }
            set { minPosition = value; }
        }

        public Vector2 MaxPosition
        {
            get { return maxPosition; }
            set { maxPosition = value; }
        }

        public Body TrackingBody
        {
            get { return trackingBody; }
            set { trackingBody = value; }
        }

        public void Update()
        {
            if (!transitioning)
            {
                if (trackingBody == null)
                {
                    if (ClampingEnabled)
                    {
                        targetPosition = Vector2.Clamp(targetPosition + new Vector2(horizontalMovement, verticalMovement), minPosition, maxPosition);
                    }
                    else
                    {
                        targetPosition += new Vector2(horizontalMovement, verticalMovement);
                    }
                }
                else
                {
                    if (ClampingEnabled)
                    {
                        targetPosition = Vector2.Clamp(ConvertUnits.ToDisplayUnits(trackingBody.Position), minPosition, maxPosition);
                    }
                    else
                    {
                        targetPosition = ConvertUnits.ToDisplayUnits(trackingBody.Position);
                    }
                }
            }
            else if (transition < 1)
            {
                transition += transitionSpeed;
            }

            if (transition >= 1f || (position == origPosition && rotation == origRotation && zoom == origZoom))
            {
                transition = 0;
                transitioning = false;
            }

            position = Vector2.SmoothStep(position, targetPosition, smoothingSpeed);
            rotation = MathHelper.SmoothStep(rotation, targetRotation, smoothingSpeed);
            zoom = MathHelper.SmoothStep(zoom, targetZoom, smoothingSpeed);
        }
    }
}