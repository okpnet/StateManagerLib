using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateManagerLib;
using StateManagerLibTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Tests
{
    [TestClass()]
    public class StateCollecionTests
    {
        StateCollecion testCollection = new();
        [TestMethod()]
        public void AddTest()
        {
            var item = new TestItem();
            testCollection.Add(item);
            item.Name = "Test1";
            item.Name = "Test2";
            item.Name = "Test3";
            item.Name = "Test4";
            testCollection.Undo();
            testCollection.Undo();
            testCollection.Undo();
            testCollection.Redo();
            Assert.IsTrue(item.Name == "Test2");
        }

        [TestMethod()]
        public void RemoveTest()
        {
            var item = new TestItem();
            testCollection.Add(item);
            item.Children = new System.Collections.ObjectModel.ObservableCollection<TestItem>();
            var child1 = new TestItem { Name = "child1" };
            var child2 = new TestItem { Name = "child2" };
            item.Children.Add(child1);
            item.Children.Add(child2);

            child1.Id = 1;
            child1.Id = 2;
            child1.Id = 3;
            child1.Id = 4;
            testCollection.Undo();
            testCollection.Undo();
            testCollection.Undo();
            testCollection.Redo();
            Assert.IsTrue(child1.Id == 2);

            item.Children.Remove(child1);
            Assert.IsTrue(testCollection.Count == 2);
        }

        [TestMethod()]
        public void InsertTest()
        {
            var item = new TestItem();
            testCollection.Add(item);
            item.Name = "Test1";
            item.Name = "Test2";
            item.Name = "Test3";
            item.Name = "Test4";
            testCollection.Undo();
            testCollection.Undo();
            testCollection.Undo();
            testCollection.Redo();
            Assert.IsTrue(item.Name == "Test2");

            item.Children = new System.Collections.ObjectModel.ObservableCollection<TestItem>();
            var child1 = new TestItem { Name = "child1" };
            var child2 = new TestItem { Name = "child2" };
            item.Children.Add(child1);
            item.Children.Add(child2);
            child1.Id = 1;
            child1.Id = 2;
            child1.Id = 3;
            child1.Id = 4;
            testCollection.Undo();
            testCollection.Undo();
            testCollection.Undo();
            testCollection.Redo();
            Assert.IsTrue(child1.Id == 2);

            testCollection.Undo(item);
            testCollection.Redo(child1);
            testCollection.Undo(item);
            Assert.IsTrue(item.Name == "Test2");

            testCollection.Insert(1, new TestItem { Name = "Insert" });
            Assert.IsTrue(testCollection.Count == 3);
        }
    }
}