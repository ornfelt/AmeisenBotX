namespace AmeisenBotX.Core.Logic.Routines
{
    public static class TrainAllSpellsRoutine
    {
        public static void Run(AmeisenBotInterfaces bot)
        {
            // Iterate through all available trainer services and buy them
            // Works for 3.3.5a
            bot.Wow.LuaDoString(@"
                local numServices = GetNumTrainerServices()
                if numServices and numServices > 0 then
                    -- Iterate backwards to handle list shifting if trained spells are removed from view
                    for i = numServices, 1, -1 do
                        local name, rank, category = GetTrainerServiceInfo(i)
                        
                        -- 'available' means we can learn it now
                        if category == 'available' then
                            BuyTrainerService(i)
                        end
                    end
                end
            ");

            // Close frame after done (optional, but good for reset)
            // bot.Wow.ClickUiElement("ClassTrainerCloseButton"); 
            // Better to let the behavior tree handle closing via movement or explicit logic, 
            // but for now we just buy spells.
        }
    }
}
