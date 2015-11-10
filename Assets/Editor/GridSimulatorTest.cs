﻿using UnityEngine;
using System.Collections;
using NUnit.Framework;
using NSubstitute;

[TestFixture]
public class GridSimulatorTest : GridTestFixture
{
    IGridSimulator gridSimulator;

    [SetUp]
    public void InitializeGridSimulator()
    {
        blockPattern = Substitute.For<IBlockPattern>();
        blockTypeMock = new BlockType[]
        {
            BlockType.One,
            BlockType.Six,
            BlockType.Three,
            BlockType.Five,
        };
        blockPattern.Types.Returns(blockTypeMock);

        group = groupFactory.Create(setting, blockPattern, groupPattern);
        
        gridSimulator = new GridSimulator(grid, setting);
    }

    [Test]
    public void ShouldCreateSameGridPatternWhenSimulate()
    {
        AddBlockMock(0, 0, 5);
        AddBlockMock(1, 0, 3);

        Assert.AreEqual(5, grid[0, 0].Number);
        Assert.AreEqual(3, grid[1, 0].Number);

        gridSimulator.CreateSimulatedGridOriginal();

        for(int y = 0; y < grid.Height; y++)
        {
            for(int x = 0; x < grid.Width; x++)
            {
                if (grid[x, y] != null)
                {
                    Assert.AreEqual(gridSimulator.SimulatedGrid[x, y].Number, grid[x, y].Number);
                }
            }
        }
    }

    [Test]
    public void ShouldDropSimulatedBlocksToBottom()
    {
        Assert.AreEqual(7, setting.GridWidth);
        Assert.AreEqual(14, setting.GridHeight);

        var blockMockOne = AddBlockMock(0, 12, 5);
        var blockMockTwo = AddBlockMock(1, 12, 3);

        gridSimulator.CreateSimulatedGridOriginal();
        Assert.IsTrue(gridSimulator.DropBlocks());

        Assert.AreEqual(blockMockOne.Number, gridSimulator.SimulatedGrid[0, 0].Number);
        Assert.AreEqual(blockMockTwo.Number, gridSimulator.SimulatedGrid[1, 0].Number);
        //Assert.AreEqual(new Coord(0, 0), gridSimulator.SimulatedGrid[0, 0].Location);
        //Assert.AreEqual(new Coord(1, 0), gridSimulator.SimulatedGrid[1, 0].Location);
    }

    [Test]
    public void ShouldResetToOriginal()
    {
        Assert.AreEqual(7, setting.GridWidth);
        Assert.AreEqual(14, setting.GridHeight);

        var blockMockOne = AddBlockMock(0, 12, 5);
        var blockMockTwo = AddBlockMock(1, 12, 3);

        gridSimulator.CreateSimulatedGridOriginal();

        Assert.AreEqual(blockMockOne.Number, gridSimulator.SimulatedGrid[0, 12].Number);
        Assert.AreEqual(blockMockTwo.Number, gridSimulator.SimulatedGrid[1, 12].Number);

        Assert.IsTrue(gridSimulator.DropBlocks());

        Assert.AreEqual(blockMockOne.Number, gridSimulator.SimulatedGrid[0, 0].Number);
        Assert.AreEqual(blockMockTwo.Number, gridSimulator.SimulatedGrid[1, 0].Number);

        gridSimulator.CopyOriginalToSimulatedGrid();

        Assert.AreEqual(blockMockOne.Number, gridSimulator.SimulatedGrid[0, 12].Number);
        Assert.AreEqual(blockMockTwo.Number, gridSimulator.SimulatedGrid[1, 12].Number);
    }

    [Test]
    public void ShouldDeleteSimulatedBlocks()
    {
        Assert.AreEqual(7, setting.GridWidth);
        Assert.AreEqual(14, setting.GridHeight);

        var blockMockOne = AddBlockMock(0, 0, 1);
        var blockMockTwo = AddBlockMock(1, 0, 2);
        var blockMockThree = AddBlockMock(2, 0, 4);

        gridSimulator.CreateSimulatedGridOriginal();

        Assert.AreEqual(blockMockOne.Number, gridSimulator.SimulatedGrid[0, 0].Number);
        Assert.AreEqual(blockMockTwo.Number, gridSimulator.SimulatedGrid[1, 0].Number);
        Assert.AreEqual(blockMockThree.Number, gridSimulator.SimulatedGrid[2, 0].Number);

        var blocksToDelete = gridSimulator.DeleteBlocks();

        Assert.IsTrue(blocksToDelete.Contains(gridSimulator.SimulatedGrid[0, 0]));
        Assert.IsTrue(blocksToDelete.Contains(gridSimulator.SimulatedGrid[1, 0]));
        Assert.IsTrue(blocksToDelete.Contains(gridSimulator.SimulatedGrid[2, 0]));
    }

    [Test]
    public void ShouldDeleteSimulatedBlocksThreeSeven()
    {
        var blockMockOne = AddBlockMock(0, 0, 7);
        var blockMockTwo = AddBlockMock(1, 0, 7);
        var blockMockThree = AddBlockMock(2, 0, 7);

        gridSimulator.CreateSimulatedGridOriginal();

        Assert.AreEqual(blockMockOne.Number, gridSimulator.SimulatedGrid[0, 0].Number);
        Assert.AreEqual(blockMockTwo.Number, gridSimulator.SimulatedGrid[1, 0].Number);
        Assert.AreEqual(blockMockThree.Number, gridSimulator.SimulatedGrid[2, 0].Number);

        var blocksToDelete = gridSimulator.DeleteBlocks();

        Assert.IsTrue(blocksToDelete.Contains(gridSimulator.SimulatedGrid[0, 0]));
        Assert.IsTrue(blocksToDelete.Contains(gridSimulator.SimulatedGrid[1, 0]));
        Assert.IsTrue(blocksToDelete.Contains(gridSimulator.SimulatedGrid[2, 0]));
    }

    [Test]
    public void ShouldSimulateGroupInTheGrid()
    {
        Assert.IsTrue(grid.AddGroup(group));

        gridSimulator.CreateSimulatedGridOriginal();

        Assert.IsNotNull(gridSimulator.SimulatedGroup);

        for(int i = 0; i < group.Children.Length; i++)
        {
            Assert.AreEqual(group.Children[i].BlockType, gridSimulator.SimulatedGroup.Children[i].BlockType);
            Assert.AreEqual(group.Children[i].Number, gridSimulator.SimulatedGroup.Children[i].Number);
            Assert.AreEqual(group.Children[i].Location, gridSimulator.SimulatedGroup.Children[i].Location);
            Assert.AreEqual(group.Children[i].LocationInTheGroup, gridSimulator.SimulatedGroup.Children[i].LocationInTheGroup);
        }
    }

    [Test]
    public void ShouldReturnScoreResultedFromTheSimulation()
    {
        Assert.IsTrue(grid.AddGroup(group));
        int expectedScore = 70;

        gridSimulator.CreateSimulatedGridOriginal();

        int result = gridSimulator.GetScoreFromSimulation();
        Assert.AreEqual(expectedScore, result);
    }

    IBlock AddBlockMock(int x, int y, int number)
    {
        IBlock blockMock = Substitute.For<IBlock>();
        blockMock.Number.Returns(number);
        blockMock.Location.Returns(new Coord(x, y));
        grid[x, y] = blockMock;

        return blockMock;
    }
}
