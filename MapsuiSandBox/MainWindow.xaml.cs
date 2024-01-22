using Mapsui.Layers;
using Mapsui.Nts.Providers;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Mapsui;
using Microsoft.UI.Xaml;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using Mapsui.Nts.Extensions;
using Mapsui.Tiling;
using System.Timers;
using Mapsui.Projections;
using Mapsui.Extensions;
using Mapsui.Providers;

namespace MapsuiSandBox;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly Map _map = new();
    private readonly ILayer _layer;
    private readonly MyLocationLayer _locationLayer;
    private readonly RasterizingTileLayer _tileLayer;

    private readonly Timer _timer = new()
    {
        Interval = 2000,
        AutoReset = true
    };

    private int _coordinateIndex = 0;
    private readonly List<MPoint> _points;

    public MainWindow()
    {
        InitializeComponent();

        _map.Layers.Add(OpenStreetMap.CreateTileLayer("demo_tile_loading_with_animation"));
        mapControl.Map = _map;

        _locationLayer = new(_map);
        _map.Layers.Add(_locationLayer);

        _timer.Elapsed += Timer_Elapsed;

        _points = CreatePoints(new MPoint(-81.2497, 42.9837)).ToList();
        _layer = CreateLayer();
        _tileLayer = new(_layer);

        _map.Layers.Add(_tileLayer);
        _map.Home = navigator => navigator.CenterOnAndZoomTo(SphericalMercator.FromLonLat(new MPoint(-81.2497, 42.9837)), _map.Navigator.Resolutions[13]);

        _timer.Start();
    }

    private MPoint[] CreatePoints(MPoint center)
    {
        var result = new MPoint[10];
        var rand = new Random();

        for (var i = 0; i < 10; i++)
        {
            result[i] = SphericalMercator.FromLonLat(center.X + rand.NextDouble() * 0.5, center.Y + rand.NextDouble() * 0.5).ToMPoint();
        }

        return result;
    }

    private List<Polygon> CreatePolygons()
    {
        List<Polygon> polygons = new List<Polygon>();

        foreach (var point in _points)
        {
            Polygon poly = new(new(new Coordinate[]
            {
                point.ToCoordinate(),
                new Coordinate(point.X + 5000, point.Y),
                new Coordinate(point.X, point.Y + 5000),
                point.ToCoordinate()
            }));
        }

        return polygons;
    }

    public static ILayer CreateLayer()
    {
        return new Layer("Polygons")
        {
            DataSource = new MemoryProvider(CreatePolygon().ToFeatures()),
            Style = new VectorStyle
            {
                Fill = new Brush(new Color(150, 150, 30, 128)),
                Outline = new Pen
                {
                    Color = Color.Orange,
                    Width = 2,
                    PenStyle = PenStyle.DashDotDot,
                    PenStrokeCap = PenStrokeCap.Round
                }
            }
        };
    }

    private static List<Polygon> CreatePolygon()
    {
        var result = new List<Polygon>();

        var polygon1 = new Polygon(
            new LinearRing(new[] {
                new Coordinate(0, 0),
                new Coordinate(0, 10000000),
                new Coordinate(10000000, 10000000),
                new Coordinate(10000000, 0),
                new Coordinate(0, 0)
            }),
            new[] {
                new LinearRing(new[] {
                    new Coordinate(1000000, 1000000),
                    new Coordinate(9000000, 1000000),
                    new Coordinate(9000000, 9000000),
                    new Coordinate(1000000, 9000000),
                    new Coordinate(1000000, 1000000)
                })
            });

        result.Add(polygon1);

        var polygon2 = new Polygon(
            new LinearRing(new[] {
                new Coordinate(-10000000, 0),
                new Coordinate(-15000000, 5000000),
                new Coordinate(-10000000, 10000000),
                new Coordinate(-5000000, 5000000),
                new Coordinate(-10000000, 0)
            }),
            new[] {
                new LinearRing(new[] {
                    new Coordinate(-10000000, 1000000),
                    new Coordinate(-6000000, 5000000),
                    new Coordinate(-10000000, 9000000),
                    new Coordinate(-14000000, 5000000),
                    new Coordinate(-10000000, 1000000)
                })
            });

        result.Add(polygon2);

        return result;
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _locationLayer.UpdateMyLocation(_points[_coordinateIndex], true);
        _coordinateIndex++;

        if (_coordinateIndex >= _points.Count)
        {
            _timer.Stop();
        }
    }
}
