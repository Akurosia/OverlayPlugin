﻿using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.InCombat
{
    public interface IInCombatMemory
    {
        bool GetInCombat();

        bool IsValid();
    }

    class InCombatMemoryManager : IInCombatMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private IInCombatMemory memory = null;

        public InCombatMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register<IInCombatMemory60, InCombatMemory60>();
            container.Register<IInCombatMemory61, InCombatMemory61>();
            repository = container.Resolve<FFXIVRepository>();
        }

        private void FindMemory()
        {
            List<IInCombatMemory> candidates = new List<IInCombatMemory>();
            // For CN, try the lang-specific candidate first, then fall back to intl
            if (repository.GetMachinaRegion() == GameRegion.Korean)
            {
                candidates.Add(container.Resolve<IInCombatMemory60>());
            }
            candidates.Add(container.Resolve<IInCombatMemory61>());

            foreach (var c in candidates)
            {
                if (c.IsValid())
                {
                    memory = c;
                    break;
                }
            }
        }

        public bool IsValid()
        {
            if (memory == null)
            {
                FindMemory();
            }
            if (memory == null || !memory.IsValid())
            {
                return false;
            }
            return true;
        }

        public bool GetInCombat()
        {
            if (!IsValid())
            {
                return false;
            }
            return memory.GetInCombat();
        }
    }
}
