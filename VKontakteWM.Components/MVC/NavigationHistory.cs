using System;
using System.Collections.Generic;

namespace Galssoft.VKontakteWM.Components.MVC
{
    internal class NavigationHistory
    {
        private LinkedList<Controller> controllerList;
        private LinkedListNode<Controller> current;

        public NavigationHistory()
        {
            this.controllerList = new LinkedList<Controller>();

        }

        public void Add(Controller controller)
        {
            if (controllerList.Contains(controller))
            {
                if (this.controllerList.Count > 0)
                {
                    this.controllerList.Remove(controller);
                }
            }

            if (current == null || this.controllerList.Count == 0)
            {
                this.controllerList.AddFirst(controller);
                this.current = controllerList.First;
            }
            else
            {
                if (this.current.Value != controller)
                {
                    try
                    {
                        this.current = controllerList.AddAfter(current, controller);
                    }
                    catch (InvalidOperationException ex)
                    {
                    }
                }
            }
        }

        public void Clear()
        {
            this.controllerList.Clear();
        }

        public Controller GetCurrent()
        {
            if (current != null)
            {
                return current.Value;
            }
            return null;
        }

        public bool CanGoForward()
        {
            if (current.Next != null)
            {
                return true;
            }
            return false;
        }

        public bool CanGoBack()
        {
            if (current.Previous != null)
            {
                return true;
            }
            return false;
        }


        public void GoForward()
        {
            LinkedListNode<Controller> node = current.Next;
            if (node != null)
            {
                this.current = node;
            }
        }

        public void GoBack()
        {
            LinkedListNode<Controller> node = current.Previous;
            //controllerList.Remove(current);
            if (node != null)
            {
                this.current = node;
            }
        }


    }
}