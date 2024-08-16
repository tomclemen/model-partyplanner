using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using MongoDB.Driver.Linq;

namespace PartyPlanner.Model;

public class GuestAgent : IAgent<GridLayer>, IPositionable
{
    #region Init

    /// <summary>
    ///     The initialization method of the GuestAgent is executed once at the beginning of a simulation.
    ///     It sets an initial Position and an initial State and generates a list of movement directions.
    /// </summary>
    /// <param name="layer">The GridLayer that manages the agents</param>
    public void Init(GridLayer layer)
    {
        _layer = layer;
        Position = new Position(StartX, StartY);
        //_directions = CreateMovementDirectionsList();
        _layer.GuestAgentEnvironment.Insert(this);
    }

    #endregion

    #region Tick

    /// <summary>
    ///     The tick method of the GuestAgent is executed during each time step of the simulation.
    ///     A GuestAgent can move randomly. It must stay within the bounds of the GridLayer and cannot move onto grid
    ///     cells that are not routable.
    ///     Near the end of the simulation, a GuestAgent removes itself from the simulation.
    /// </summary>
    public void Tick()  
    {
        // Update 
        OtherGuests = ExploreOtherGuests();

        // MoveRandomly();
        //
        // if (_layer.GetCurrentTick() == 595)
        // {
        //     RemoveFromSimulation();
        // }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generates a list of other party guests
    /// </summary>

    // return new List<FriendSnapshot>(Battleground.FighterEnv
    // .Entities.Where(body => body.Color == Color && body != this).Select(b =>
    //     new FriendSnapshot(b.ID, b.MemberId, b.Color, b.Stance, b.Position, b.Energy, b.VisualRange, b.VisibilityRange))
    // .ToList());
    
    public List<GuestAgent> ExploreOtherGuests()
    {
        //return new List<GuestAgent>(_layer.GuestAgentEnvironment.Entities.Where(GuestAgent => ID != this.ID )).ToList();
        return _layer.GuestAgentEnvironment.Explore().ToList();
    }
    
    /// <summary>
    ///     Generates a list of eight movement directions that the agent uses for random movement.
    /// </summary>
    /// <returns>The list of movement directions</returns>
    private static List<Position> CreateMovementDirectionsList()
    {
        return new List<Position>
        {
            MovementDirections.North,
            MovementDirections.Northeast,
            MovementDirections.East,
            MovementDirections.Southeast,
            MovementDirections.South,
            MovementDirections.Southwest,
            MovementDirections.West,
            MovementDirections.Northwest
        };
    }
    
    /// <summary>
    ///     Removes this agent from the simulation and, by extension, from the visualization.
    /// </summary>
    private void RemoveFromSimulation()
    {
        Console.WriteLine($"GuestAgent {ID} is removing itself from the simulation.");
        _layer.GuestAgentEnvironment.Remove(this);
        UnregisterAgentHandle.Invoke(_layer, this);
    }

    /// <summary>
    ///     Performs one random move, if possible, using the movement directions list.
    /// </summary>
    private void MoveRandomly()
    {
        var nextDirection = _directions[_random.Next(_directions.Count)];
        var newX = Position.X + nextDirection.X;
        var newY = Position.Y + nextDirection.Y;

        // Check if chosen move is within the bounds of the grid
        if (0 <= newX && newX < _layer.Width && 0 <= newY && newY < _layer.Height)
        {
            // Check if chosen move goes to a cell that is routable
            if (_layer.IsRoutable(newX, newY))
            {
                Position = new Position(newX, newY);
                _layer.GuestAgentEnvironment.MoveTo(this, new Position(newX, newY));
                Console.WriteLine("{0} moved to a new cell: {1}", GetType().Name, Position);
            }
            else
            {
                Console.WriteLine("{0} tried to move to a blocked cell: ({1}, {2})", GetType().Name, newX, newY);
            }
        }
        else
        {
            Console.WriteLine("{0} tried to leave the world: ({1}, {2})", GetType().Name, newX, newY);
        }
    }

    /// <summary>
    ///     Increments the agent's MeetingCounter property value.
    /// </summary>
    public void IncrementCounter()
    {
        MeetingCounter += 1;
    }

    #endregion

    #region Fields and Properties

    public Guid ID { get; set; }
    
    public Position Position { get; set; }
    
    private List<GuestAgent> OtherGuests { get; set; }
    
    [PropertyDescription(Name = "Name")]
    public string Name { get; set; }
    
    [PropertyDescription(Name = "Occupation")]
    public string Occupation { get; set; }
    
    [PropertyDescription(Name = "StartX")]
    public int StartX { get; set; }
    
    [PropertyDescription(Name = "StartY")]
    public int StartY { get; set; }

    [PropertyDescription(Name = "A")]
    public int TargetDistA { get; set; }
    
    [PropertyDescription(Name = "B")]
    public int TargetDistB { get; set; }

    [PropertyDescription(Name = "D")]
    public int TargetDistD { get; set; }

    [PropertyDescription(Name = "M")]
    public int TargetDistM { get; set; }

    [PropertyDescription(Name = "P")]
    public int TargetDistP { get; set; }

    [PropertyDescription(Name = "S")]
    public int TargetDistS { get; set; }

    [PropertyDescription(Name = "V")]
    public int TargetDistV { get; set; }

    [PropertyDescription(Name = "W")]
    public int TargetDistW { get; set; }

    public int MeetingCounter { get; private set; }

    public UnregisterAgent UnregisterAgentHandle { get; set; }
    
    private GridLayer _layer;
    private List<Position> _directions;
    private readonly Random _random = new();

    #endregion
}