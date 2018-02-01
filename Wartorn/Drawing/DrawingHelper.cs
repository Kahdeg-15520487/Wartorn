using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Wartorn {
	namespace Drawing {
		public static class DrawingHelper {
			public static PrimitiveBatch primitiveBatch;

			public static void Initialize(GraphicsDevice device) {
				DrawingHelper.primitiveBatch = new PrimitiveBatch(device);
			}

			public static void Begin(PrimitiveType primitiveType) {
				DrawingHelper.primitiveBatch.Begin(primitiveType);
			}

			public static void End() {
				DrawingHelper.primitiveBatch.End();
			}

			public static void DrawPoint(Vector2 vertex, Color color) {
				DrawingHelper.primitiveBatch.AddVertex(vertex, color);
			}

			public static void DrawLine(Vector2 start, Vector2 end, Color color) {
				DrawingHelper.primitiveBatch.AddVertex(start, color);
				DrawingHelper.primitiveBatch.AddVertex(end, color);
			}

			public static void DrawRectangle(Rectangle rect, Color color, bool fill) {
				if (fill) {
					DrawingHelper.primitiveBatch.Begin(PrimitiveType.TriangleStrip);
					DrawingHelper.primitiveBatch.AddVertex(rect.Left, rect.Top, color);
					DrawingHelper.primitiveBatch.AddVertex(rect.Right, rect.Top, color);
					DrawingHelper.primitiveBatch.AddVertex(rect.Left, rect.Bottom, color);
					DrawingHelper.primitiveBatch.AddVertex(rect.Right, rect.Bottom, color);
					DrawingHelper.primitiveBatch.End();
					return;
				}
				DrawingHelper.primitiveBatch.Begin(PrimitiveType.LineList);
				DrawingHelper.primitiveBatch.AddVertex(rect.X, rect.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(rect.X + rect.Width, rect.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(rect.X + rect.Width, rect.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(rect.X + rect.Width, rect.Y + rect.Height, color);
				DrawingHelper.primitiveBatch.AddVertex(rect.X + rect.Width, rect.Y + rect.Height, color);
				DrawingHelper.primitiveBatch.AddVertex(rect.X, rect.Y + rect.Height, color);
				DrawingHelper.primitiveBatch.AddVertex(rect.X, rect.Y + rect.Height, color);
				DrawingHelper.primitiveBatch.AddVertex(rect.X, rect.Y, color);
				DrawingHelper.primitiveBatch.End();
			}

			public static void DrawRectangle(Vector2 position, Vector2 size, Color color, bool fill) {
				if (fill) {
					DrawingHelper.primitiveBatch.Begin(PrimitiveType.TriangleStrip);
					DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y, color);
					DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y, color);
					DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y + size.Y, color);
					DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y + size.Y, color);
					DrawingHelper.primitiveBatch.End();
					return;
				}
				DrawingHelper.primitiveBatch.Begin(PrimitiveType.LineList);
				DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y + size.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y + size.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y + size.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y + size.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y, color);
				DrawingHelper.primitiveBatch.End();
			}

			public static void DrawRectangle(Point position, Point size, Color color, bool fill) {
				if (fill) {
					DrawingHelper.primitiveBatch.Begin(PrimitiveType.TriangleStrip);
					DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y, color);
					DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y, color);
					DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y + size.Y, color);
					DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y + size.Y, color);
					DrawingHelper.primitiveBatch.End();
					return;
				}
				DrawingHelper.primitiveBatch.Begin(PrimitiveType.LineList);
				DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y + size.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X + size.X, position.Y + size.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y + size.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y + size.Y, color);
				DrawingHelper.primitiveBatch.AddVertex(position.X, position.Y, color);
				DrawingHelper.primitiveBatch.End();
			}

			public static void DrawRectangle(int x1, int y1, int x2, int y2, Color color, bool fill) {
				if (fill) {
					DrawingHelper.primitiveBatch.Begin(PrimitiveType.TriangleStrip);
					DrawingHelper.primitiveBatch.AddVertex(x1, y1, color);
					DrawingHelper.primitiveBatch.AddVertex(x2, y1, color);
					DrawingHelper.primitiveBatch.AddVertex(x1, y2, color);
					DrawingHelper.primitiveBatch.AddVertex(x2, y2, color);
					DrawingHelper.primitiveBatch.End();
					return;
				}
				DrawingHelper.primitiveBatch.Begin(PrimitiveType.LineList);
				DrawingHelper.primitiveBatch.AddVertex(x1, y1, color);
				DrawingHelper.primitiveBatch.AddVertex(x2, y1, color);
				DrawingHelper.primitiveBatch.AddVertex(x2, y1, color);
				DrawingHelper.primitiveBatch.AddVertex(x2, y2, color);
				DrawingHelper.primitiveBatch.AddVertex(x2, y2, color);
				DrawingHelper.primitiveBatch.AddVertex(x1, y2, color);
				DrawingHelper.primitiveBatch.AddVertex(x1, y2, color);
				DrawingHelper.primitiveBatch.AddVertex(x1, y1, color);
				DrawingHelper.primitiveBatch.End();
			}

			public static void DrawRectangle(float x1, float y1, float x2, float y2, Color color, bool fill) {
				if (fill) {
					DrawingHelper.primitiveBatch.Begin(PrimitiveType.TriangleStrip);
					DrawingHelper.primitiveBatch.AddVertex(x1, y1, color);
					DrawingHelper.primitiveBatch.AddVertex(x2, y1, color);
					DrawingHelper.primitiveBatch.AddVertex(x1, y2, color);
					DrawingHelper.primitiveBatch.AddVertex(x2, y2, color);
					DrawingHelper.primitiveBatch.End();
					return;
				}
				DrawingHelper.primitiveBatch.Begin(PrimitiveType.LineList);
				DrawingHelper.primitiveBatch.AddVertex(x1, y1, color);
				DrawingHelper.primitiveBatch.AddVertex(x2, y1, color);
				DrawingHelper.primitiveBatch.AddVertex(x2, y1, color);
				DrawingHelper.primitiveBatch.AddVertex(x2, y2, color);
				DrawingHelper.primitiveBatch.AddVertex(x2, y2, color);
				DrawingHelper.primitiveBatch.AddVertex(x1, y2, color);
				DrawingHelper.primitiveBatch.AddVertex(x1, y2, color);
				DrawingHelper.primitiveBatch.AddVertex(x1, y1, color);
				DrawingHelper.primitiveBatch.End();
			}

			public static void DrawCircle(Vector2 center, float radius, Color color, bool fill) {
				if (fill) {
					DrawingHelper.DrawNGon(center, radius, 663, color, fill);
					return;
				}
				DrawingHelper.DrawNGon(center, radius, 997, color, fill);
			}

			public static void DrawNGon(Vector2 center, float radius, int numSides, Color color, bool fill) {
				if (numSides < 3) {
					return;
				}
				if (radius == 0f) {
					DrawingHelper.primitiveBatch.Begin(PrimitiveType.LineStrip);
					DrawingHelper.primitiveBatch.AddVertex(center, color);
				}
				else {
					if (fill) {
						DrawingHelper.primitiveBatch.Begin(PrimitiveType.TriangleStrip);
					}
					else {
						DrawingHelper.primitiveBatch.Begin(PrimitiveType.LineStrip);
					}
					float num = 6.28318548f;
					float num2 = num / (float)numSides;
					int num3 = 0;
					for (float num4 = 0f; num4 <= num; num4 += num2) {
						if (fill && num3 % 3 == 0) {
							DrawingHelper.primitiveBatch.AddVertex(center.X, center.Y, color);
						}
						DrawingHelper.primitiveBatch.AddVertex(center.X + radius * (float)Math.Cos((double)num4), center.Y + radius * (float)Math.Sin((double)num4), color);
						num3++;
					}
				}
				DrawingHelper.primitiveBatch.End();
			}

			public static void DrawFastLine(Vector2 start, Vector2 end, Color color) {
				DrawingHelper.primitiveBatch.Begin(PrimitiveType.LineList);
				DrawingHelper.primitiveBatch.AddVertex(start, color);
				DrawingHelper.primitiveBatch.AddVertex(end, color);
				DrawingHelper.primitiveBatch.End();
			}
		}
	}
}
