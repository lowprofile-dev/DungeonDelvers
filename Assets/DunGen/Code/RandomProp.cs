using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen
{
	public abstract class RandomProp : MonoBehaviour
	{
        public virtual void Process(System.Random randomStream, Tile tile) { }
	}
}
