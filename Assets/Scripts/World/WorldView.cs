using BoatGame.Boat;
using UnityEngine;

public class WorldView : MonoBehaviour
{
    [SerializeField] private Transform upperLeft;
    [SerializeField] private Transform upperMiddle;
    [SerializeField] private Transform upperRight;
    [SerializeField] private Transform middleLeft;
    [SerializeField] private Transform middleMiddle;
    [SerializeField] private Transform middleRight;
    [SerializeField] private Transform lowerLeft;
    [SerializeField] private Transform lowerMiddle;
    [SerializeField] private Transform lowerRight;

    [SerializeField] private WeatherEngineScriptableObject weatherEngineScriptableObject;
    [SerializeField] private BoatSimulationModel boatSimulationModel;
    
    private Transform[,] _tiles;
    private Vector3 _columnStep;
    private Vector3 _rowStep;

    private void Awake()
    {
        weatherEngineScriptableObject.StartSimulation();
        
        _tiles = new[,]
        {
            { lowerLeft, middleLeft, upperLeft },
            { lowerMiddle, middleMiddle, upperMiddle },
            { lowerRight, middleRight, upperRight }
        };
        _columnStep = Flat(middleRight.position - middleMiddle.position);
        _rowStep = Flat(upperMiddle.position - middleMiddle.position);
    }

    private void Update()
    {
        var offset = Flat(new Vector3(boatSimulationModel.BoatPosition.x, 0f, boatSimulationModel.BoatPosition.y) -
                          _tiles[1, 1].position);
        var stepColumn = Mathf.RoundToInt(Vector3.Dot(offset, _columnStep) / _columnStep.sqrMagnitude);
        var stepRow = Mathf.RoundToInt(Vector3.Dot(offset, _rowStep) / _rowStep.sqrMagnitude);
        stepColumn = Mathf.Clamp(stepColumn, -1, 1);
        stepRow = Mathf.Clamp(stepRow, -1, 1);
        if (stepColumn == 0 && stepRow == 0) return;

        var next = new Transform[3, 3];
        for (var c = 0; c < 3; c++)
        for (var r = 0; r < 3; r++)
        {
            var nc = ((c - stepColumn) % 3 + 3) % 3;
            var nr = ((r - stepRow) % 3 + 3) % 3;
            var tile = _tiles[c, r];
            var wrapColumn = Mathf.Abs(c - nc) == 2 ? (nc - c) * 1.5f : 0f;
            var wrapRow = Mathf.Abs(r - nr) == 2 ? (nr - r) * 1.5f : 0f;
            tile.position += _columnStep * wrapColumn + _rowStep * wrapRow;
            next[nc, nr] = tile;
        }

        _tiles = next;
        
        var shift = -(_columnStep * stepColumn + _rowStep * stepRow);
        foreach (var tile in _tiles) tile.position += shift;
        boatSimulationModel.ShiftBoatPosition(new Vector2(shift.x, shift.z));
    }

    private static Vector3 Flat(Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }
}