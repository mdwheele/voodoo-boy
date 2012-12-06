using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Artemis
{
	public enum ExecutionType
    {
        Draw,
        Update
    }
	
	public sealed class SystemManager {
		private EntityWorld world;
        private Dictionary<Type, List<EntitySystem>> systems = new Dictionary<Type, List<EntitySystem>>();

        private Dictionary<int, Bag<EntitySystem>> Updatelayers = new Dictionary<int, Bag<EntitySystem>>();
        private Dictionary<int, Bag<EntitySystem>> Drawlayers = new Dictionary<int, Bag<EntitySystem>>();
		private Bag<EntitySystem> mergedBag = new Bag<EntitySystem>();
		
		internal SystemManager(EntityWorld world) {
			this.world = world;
		}
		
		public T SetSystem<T>(T system,ExecutionType execType , int layer = 0) where T : EntitySystem {
			system.World = world;
						
            if(systems.ContainsKey(typeof(T)))
            {
                systems[typeof(T)].Add((EntitySystem)system);
            }
            else
            {
                systems[typeof(T)] = new List<EntitySystem>();
                systems[typeof(T)].Add((EntitySystem)system);
            }

			
			if(execType == ExecutionType.Draw) {

                if (!Drawlayers.ContainsKey(layer))
                    Drawlayers[layer] = new Bag<EntitySystem>();

                Bag<EntitySystem> drawBag = Drawlayers[layer];
                if (drawBag == null)
                {
                    drawBag = Drawlayers[layer] = new Bag<EntitySystem>();
                }
				if(!drawBag.Contains((EntitySystem)system))
					drawBag.Add((EntitySystem)system);
				Drawlayers = (from d in Drawlayers orderby d.Key ascending select d).ToDictionary(pair => pair.Key, pair => pair.Value);
			} else if(execType == ExecutionType.Update) {

                if (!Updatelayers.ContainsKey(layer))
                    Updatelayers[layer] = new Bag<EntitySystem>();                

                Bag<EntitySystem> updateBag = Updatelayers[layer];
                if (updateBag == null)
                {
                    updateBag = Updatelayers[layer] = new Bag<EntitySystem>();
                }
				if(!updateBag.Contains((EntitySystem)system))
					updateBag.Add((EntitySystem)system);
				Updatelayers = (from d in Updatelayers orderby d.Key ascending select d).ToDictionary(pair => pair.Key, pair => pair.Value);
			}
			if(!mergedBag.Contains((EntitySystem)system))
					mergedBag.Add((EntitySystem)system);
			system.SystemBit = SystemBitManager.GetBitFor(system);
			
			return (T)system;
		}
		
		public List<EntitySystem> GetSystem<T>() where T : EntitySystem {
            List<EntitySystem> system;

            systems.TryGetValue(typeof(T), out system);

            return system;
		}
		
		public Bag<EntitySystem> Systems {
			get { return mergedBag;}
		}
		
		/**
		 * After adding all systems to the world, you must initialize them all.
		 */
		public void InitializeAll() {
		   for (int i = 0, j = mergedBag.Size; i < j; i++) {
		      mergedBag.Get(i).Initialize();
		   }
		}


        void UpdatebagSync(Bag<EntitySystem> temp) 
        {
            for (int i = 0, j = temp.Size; i < j; i++)
            {
                temp.Get(i).Process();
            }             
        }
		
		public void UpdateSynchronous(ExecutionType execType )
        {            
			if(execType == ExecutionType.Draw) {

                foreach (int item in Drawlayers.Keys)                
                {
                    UpdatebagSync(Drawlayers[item]);
                } 				
			} else if(execType == ExecutionType.Update) {
                foreach (int item in Updatelayers.Keys)
                {
                    UpdatebagSync(Updatelayers[item]);
                } 
			}	
        }

        TaskFactory factory = new TaskFactory(TaskScheduler.Default);
        List<Task> tasks = new List<Task>();

        void UpdatebagASSync(Bag<EntitySystem> temp)
        {
            tasks.Clear();
            for (int i = 0, j = temp.Size; i < j; i++)
            {
                EntitySystem es = temp.Get(i);
                tasks.Add(factory.StartNew(
                    () =>
                    {
                        es.Process();
                    }
                ));

            }
            Task.WaitAll(tasks.ToArray());
        }
        public void UpdateAsynchronous(ExecutionType execType )
        {
            if (execType == ExecutionType.Draw)
            {
                foreach (int item in Drawlayers.Keys)
                {
                    UpdatebagASSync(Drawlayers[item]);
                }
            }
            else if (execType == ExecutionType.Update)
            {
                foreach (int item in Updatelayers.Keys)
                {
                    UpdatebagASSync(Updatelayers[item]);
                }
            }	
            
        }
		
	}
}