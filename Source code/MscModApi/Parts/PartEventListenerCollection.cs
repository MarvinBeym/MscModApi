using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MscModApi.Parts
{
	/// <summary>
	/// Collection of PartEventListener objects
	/// </summary>
	public class PartEventListenerCollection
	{
		/// <summary>
		/// Flag set when executing InvokeAll method,
		/// if set to true, PartEventListeners removed using the Remove method will not be executed in the InvokeAll and removed after the InvokeAll is finished
		/// </summary>
		protected bool currentlyIterating;

		/// <summary>
		/// Protected writable list of PartEventListeners
		/// </summary>
		protected List<PartEventListener> _eventListeners;

		/// <summary>
		/// Returns a read only list of event listeners
		/// </summary>
		public ReadOnlyCollection<PartEventListener> eventListeners =>
			new ReadOnlyCollection<PartEventListener>(_eventListeners);

		/// <summary>
		/// A collection of PartEventListeners
		/// </summary>
		/// <param name="eventListeners">Initial list of PartEventListeners</param>
		public PartEventListenerCollection(List<PartEventListener> eventListeners)
		{
			_eventListeners = eventListeners;
		}

		/// <summary>
		/// A collection of PartEventListeners
		/// </summary>
		public PartEventListenerCollection()
		{
			_eventListeners = new List<PartEventListener>();
		}

		/// <summary>
		/// Adds a new PartEventListener to this collection
		/// </summary>
		/// <param name="partEventListener">The PartEventListener object to add to the collection</param>
		public void Add(PartEventListener partEventListener)
		{
			_eventListeners.Add(partEventListener);
		}

		/// <summary>
		/// Invokes all PartEventListeners, allows PartEventListeners to be removed inside of a PartEventListeners
		/// </summary>
		public void InvokeAll()
		{
			currentlyIterating = true;

			try
			{
				_eventListeners.ForEach((partEventListener =>
				{
					if (!partEventListener.delete)
					{
						partEventListener.action.Invoke();
					}
				}));
			}
			catch
			{
				currentlyIterating = false;
			}

			currentlyIterating = false;

			for (int i = _eventListeners.Count - 1; i >= 0; i--)
			{
				if (_eventListeners[i].delete)
				{
					_eventListeners.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Returns if the collection contains a specific PartEventListener
		/// </summary>
		/// <param name="partEventListener">The PartEventListener object to check</param>
		/// <returns>True if the collection contains the provided PartEventListener, otherwise false</returns>
		public bool Contains(PartEventListener partEventListener)
		{
			return _eventListeners.Contains(partEventListener);
		}

		/// <summary>
		/// Removes a PartEventListener object from the collection
		/// If the InvokeAll is currently being run, the PartEventListener will not be executed (if it hasn't already) and removed after the InvokeAll is finished
		/// </summary>
		/// <param name="partEventListener">The PartEventListener to remove</param>
		/// <returns>True if the PartEventListener was removed (or will be removed)</returns>
		public bool Remove(PartEventListener partEventListener)
		{
			if (!Contains(partEventListener))
			{
				return false;
			}

			if (!currentlyIterating)
			{
				return _eventListeners.Remove(partEventListener);
			}

			partEventListener.delete = true;
			return true;
		}
	}
}