using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Numerics;

namespace cycling_map
{
    internal class PointsComputation
    {
        public static List<Location> collectRoutePoints(Route RouteInfo)
        {
            var RoutePoints = new List<Location>();

            foreach (var point in RouteInfo.Legs[0].Points)
            {
                RoutePoints.Add(new Location(point.Latitude, point.Longitude));
            }

            return RoutePoints;
        }
        public static RenderTargetBitmap DrawRoute(BitmapImage tile, List<Location> RoutePoints, Location firstPoint,
        Location secondPoint, int _zoomLevel)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(tile, new Rect(0, 0, tile.PixelWidth, tile.PixelHeight));
                if (RoutePoints.Count > 0)
                {
                    for (int i = 0; i + 1 < RoutePoints.Count; i++)
                    {
                        DrawLine(drawingContext, RoutePoints[i], RoutePoints[i + 1], _zoomLevel, Colors.Indigo);
                    }
                }

                if (firstPoint != null) DrawPoint(drawingContext, firstPoint, _zoomLevel, Colors.Blue);

                if (secondPoint != null) DrawPoint(drawingContext, secondPoint, _zoomLevel, Colors.Red);
            }

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(tile.PixelWidth, tile.PixelHeight, tile.DpiX,
                tile.DpiY, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            return renderBitmap;
        }

        private static void DrawPoint(DrawingContext drawingContext, Location point, int zoom, Color color)
        {
            var (pixelX, pixelY) = MathCalculations.LatLonToPixelXY(point.Lat(), point.Lon(), zoom);
            var (localX, localY) = (pixelX % 512, pixelY % 512);

            drawingContext.DrawEllipse(new SolidColorBrush(color), null, new System.Windows.Point(localX, localY), 3, 3);
        }

        private static void DrawLine(DrawingContext drawingContext, Location point1, Location point2, int zoom, Color color)
        {
            var (pixelX1, pixelY1) = MathCalculations.LatLonToPixelXY(point1.Lat(), point1.Lon(), zoom);
            var (localX1, localY1) = (pixelX1 % 512, pixelY1 % 512);
            var (pixelX2, pixelY2) = MathCalculations.LatLonToPixelXY(point2.Lat(), point2.Lon(), zoom);
            var (localX2, localY2) = (pixelX2 % 512, pixelY2 % 512);

            drawingContext.DrawLine(new Pen(new SolidColorBrush(color), 2), new System.Windows.Point(localX1, localY1),
                null, new System.Windows.Point(localX2, localY2), null);
        }
        private struct BoundingBox
        {
            public double left, right, top, bottom;

            public BoundingBox(double left, double right, double top, double bottom)
            {
                this.left = left;
                this.right = right;
                this.top = top;
                this.bottom = bottom;
            }
        }

        public static (int,int, int) calculateBoundingBox(Location firstPoint, Location secondPoint, List<Location> RoutePoints)
        {
            var bbox = new BoundingBox(firstPoint.Lon(), firstPoint.Lon(), firstPoint.Lat(), firstPoint.Lat());
            foreach (var point in RoutePoints)
            {
                if (point.Lat() > bbox.top) bbox.top = point.Lat();
                if (point.Lat() < bbox.bottom) bbox.bottom = point.Lat();

                if (point.Lon() < bbox.left) bbox.left = point.Lon();

                if (point.Lon() > bbox.right) bbox.right = point.Lon();
            }

            if (secondPoint.Lat() > bbox.top) bbox.top = secondPoint.Lat();

            if (secondPoint.Lat() < bbox.bottom) bbox.bottom = secondPoint.Lat();

            if (secondPoint.Lon() < bbox.left) bbox.left = secondPoint.Lon();

            if (secondPoint.Lon() > bbox.right) bbox.right = secondPoint.Lon();


            double bboxWidthDeg = bbox.right - bbox.left;
            double bboxHeightDeg = bbox.top - bbox.bottom;

            double bboxWidthT = (1 - Math.Cos(bboxWidthDeg)) / 2 * 6371000;
            double bboxHeightT = (1 - Math.Cos(bboxHeightDeg)) / 2 * 6371000;

            double bboxSize = Math.Max(bboxWidthT, bboxHeightT);

            int _zoomLevel = 22;
            var topleft = new Location(bbox.top, bbox.left);
            var (tlx, tly) = MathCalculations.calculateLonLatToXY(topleft, _zoomLevel);
            var botright = new Location(bbox.bottom, bbox.right);
            var (brx, bry) = MathCalculations.calculateLonLatToXY(botright, _zoomLevel);
            while ((tlx != brx || tly != bry) && _zoomLevel > 0)
            {
                _zoomLevel--;
                (tlx, tly) = MathCalculations.calculateLonLatToXY(topleft, _zoomLevel);
                (brx, bry) = MathCalculations.calculateLonLatToXY(botright, _zoomLevel);
            }
            int _tileX, _tileY;
            (_tileX, _tileY) = MathCalculations.calculateLonLatToXY(botright, _zoomLevel);
            return (_tileX, _tileY, _zoomLevel);
        }
    }
}
