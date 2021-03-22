namespace TestMVVM
{
	using System;
	using System.Collections.Generic;
	using Crowdoka.DataBinding;
	using OneOf;
	using UniRx;
	using UnityEngine;

	public class Screen1ViewModel : ViewModelBase<Screen1Model>
    {
        public void SetToPublic()
        {
            Data.TestE = TestEnum.Public;
        }

        public void SetToPrivate()
        {
            Data.TestE = TestEnum.Private;
        }

        public void SetTo(TestEnum testEnum)
        {
            Data.TestE = testEnum;
        }

        public void AddToList()
		{
            Data.TestList.Add(new SubModel
            {
                Name = $"Test{UnityEngine.Random.Range(0, 500)}"
            });
        }

        public void RemoveRandom()
        {
            var index = UnityEngine.Random.Range(0, Data.TestList.Count);
            Data.TestList.RemoveAt(index);
        }
    }

    public class Screen1Model
    {
        public List<SubModel> TestList { get; set; } = new List<SubModel>()
        {
            new SubModel
			{
                Name = "Test"
			},
            new SubModel
			{
                Name = "Toto"
			}
        };

        public List<string> TestDropdown { get; set; } = new List<string>()
        {
            "Option 1",
            "Option 2",
            "Option 3",
            "Option 4",
        };

        public DateTime TestDateTime { get; set; } = DateTime.Now;

        public float Value { get; set; }

        public TestObject TestO { get; set; } = new TestObject
        {
            Name = "Test of object"
        };

        public TestEnum TestE { get; set; }

        public OneOf<Color, string> BaseColor { get; set; } = "cyan";
    }

    public enum TestEnum
    {
        Public, Protected, Private, Internal, None
    }
}