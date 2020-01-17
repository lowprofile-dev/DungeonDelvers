using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen
{
	[Serializable]
	public class Door : MonoBehaviour
	{
		public delegate void DoorStateChangedDelegate(Door door, bool isOpen);

		[HideInInspector]
		public Dungeon Dungeon;
		[HideInInspector]
		public Doorway DoorwayA;
		[HideInInspector]
		public Doorway DoorwayB;
		[HideInInspector]
		public Tile TileA;
		[HideInInspector]
		public Tile TileB;

		public virtual bool IsOpen
		{
			get { return isOpen; }
			set
			{
				if (isOpen == value)
					return;

				SetDoorState(value);
			}
		}

		public event DoorStateChangedDelegate OnDoorStateChanged;


		[SerializeField]
		private bool isOpen;


		private void OnDestroy()
		{
			OnDoorStateChanged = null;
		}

		public void SetDoorState(bool isOpen)
		{
			if (this.isOpen == isOpen)
				return;

			this.isOpen = isOpen;

			if (OnDoorStateChanged != null)
				OnDoorStateChanged(this, isOpen);
		}
	}
}
