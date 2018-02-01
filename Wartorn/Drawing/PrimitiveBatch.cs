using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Wartorn {
	namespace Drawing {
		public class PrimitiveBatch : IDisposable {
			public const int DefaultBufferSize = 1000;

			private VertexPositionColor[] vertices = new VertexPositionColor[1000];

			private int positionInBuffer;

			private BasicEffect basicEffect;

			private GraphicsDevice device;

			private PrimitiveType primitiveType;

			private int numVertsPerPrimitive;

			private bool hasBegun;

			private bool isDisposed;

			public PrimitiveBatch(GraphicsDevice graphicsDevice) {
				if (graphicsDevice == null) {
					throw new ArgumentNullException("graphicsDevice");
				}
				this.device = graphicsDevice;
				this.basicEffect = new BasicEffect(graphicsDevice);
				this.basicEffect.VertexColorEnabled = true;
				this.basicEffect.LightingEnabled = false;
				BlendState blendState = new BlendState();
				blendState.AlphaSourceBlend = Blend.SourceAlpha;
				blendState.AlphaBlendFunction = BlendFunction.Add;
				blendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
				this.device.BlendState = blendState;
				this.basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0f, (float)graphicsDevice.Viewport.Width, (float)graphicsDevice.Viewport.Height, 0f, 0f, 1f);
			}

			public void Dispose() {
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing) {
				if (disposing && !this.isDisposed) {
					if (this.basicEffect != null) {
						this.basicEffect.Dispose();
					}
					this.isDisposed = true;
				}
			}

			public void Begin(PrimitiveType primitiveType) {
				if (this.hasBegun) {
					throw new InvalidOperationException("End must be called before Begin can be called again.");
				}
				this.primitiveType = primitiveType;
				this.numVertsPerPrimitive = PrimitiveBatch.NumVertsPerPrimitive(primitiveType);
				this.basicEffect.CurrentTechnique.Passes[0].Apply();
				this.hasBegun = true;
			}

			public void AddVertex(Vector2 vertex, Color color) {
				if (!this.hasBegun) {
					throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
				}
				bool flag = this.positionInBuffer % this.numVertsPerPrimitive == 0;
				if (flag && this.positionInBuffer + this.numVertsPerPrimitive >= this.vertices.Length) {
					this.Flush();
				}
				this.vertices[this.positionInBuffer].Position = new Vector3(vertex, 0f);
				this.vertices[this.positionInBuffer].Color = color;
				this.positionInBuffer++;
			}

			public void AddVertex(int x, int y, Color color) {
				this.AddVertex(new Vector2((float)x, (float)y), color);
			}

			public void AddVertex(float x, float y, Color color) {
				this.AddVertex(new Vector2(x, y), color);
			}

			public void End() {
				if (!this.hasBegun) {
					throw new InvalidOperationException("Begin must be called before End can be called.");
				}
				this.Flush();
				this.hasBegun = false;
			}

			private void Flush() {
				if (!this.hasBegun) {
					throw new InvalidOperationException("Begin must be called before Flush can be called.");
				}
				if (this.positionInBuffer == 0) {
					return;
				}
				int primitiveCount = 0;
				switch (this.primitiveType) {
					case PrimitiveType.TriangleList:
						primitiveCount = this.positionInBuffer / 3;
						break;
					case PrimitiveType.TriangleStrip:
						primitiveCount = this.positionInBuffer - 2;
						break;
					case PrimitiveType.LineList:
						primitiveCount = this.positionInBuffer / 2;
						break;
					case PrimitiveType.LineStrip:
						primitiveCount = this.positionInBuffer - 1;
						break;
				}
				this.device.DrawUserPrimitives<VertexPositionColor>(this.primitiveType, this.vertices, 0, primitiveCount);
				this.positionInBuffer = 0;
			}

			private static int NumVertsPerPrimitive(PrimitiveType primitive) {
				int result;
				switch (primitive) {
					case PrimitiveType.TriangleList:
					case PrimitiveType.TriangleStrip:
						result = 3;
						break;
					case PrimitiveType.LineList:
					case PrimitiveType.LineStrip:
						result = 2;
						break;
					default:
						throw new InvalidOperationException("primitive is not valid");
				}
				return result;
			}
		}
	}
}
