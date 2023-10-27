namespace AmeisenBotX.Wow.Shared.Lua
{
    /// <summary>
    /// Represents a static class that provides methods for retrieving data from a frame, table, handler function, and output string.
    /// </summary>
    public static class LuaEventHook
    {
        /// <summary>
        /// Retrieves data from a frame, table, handler function, and output string.
        /// </summary>
        /// <param name="frame">The frame to get data from.</param>
        /// <param name="table">The table to store data in.</param>
        /// <param name="handlerFn">The handler function for events.</param>
        /// <param name="output">The output string to store the final result.</param>
        /// <returns>A formatted JSON-like string representing the data.</returns>
        public static string Get(string frame, string table, string handlerFn, string output)
        {
            return @$"
                {output}='['

                function {handlerFn}(self,a,...)
                    table.insert({table},{{time(),a,{{...}}}})
                end

                if {frame}==nil then
                    {table}={{}}
                    {frame}=CreateFrame(""FRAME"")
                    {frame}:SetScript(""OnEvent"",{handlerFn})
                else
                    for b,c in pairs({table})do
                        {output}={output}..'{{'

                        for d,e in pairs(c)do
                            if type(e)==""table""then
                                {output}={output}..'""args"": ['

                                for f,g in pairs(e)do
                                    {output}={output}..'""'..tostring(g)..'""'

                                    if f<=table.getn(e)then
                                        {output}={output}..','
                                    end
                                end

                                {output}={output}..']}}'

                                if b<table.getn({table})then
                                    {output}={output}..','
                                end
                            else
                                if type(e)==""string""then
                                    {output}={output}..'""event"": ""'..e..'"",'
                                else
                                    {output}={output}..'""time"": ""'..e..'"",'
                                end
                            end
                        end
                    end
                end

                {output}={output}..']'
                {table}={{}}
            ";
        }
    }
}