namespace IceMachineDriverLibrary.IceMachine.Nakazo.Service;

public static class NakazoDataUtils
{
    /// <summary>
        /// Converts Temperature data (hex string, e.g. 0x10) to integer
        /// </summary>
        /// <param name="temperature"> Hex string of temperature data received from the Ice Machine</param>
        /// <returns>Converted Temperature data in degree celsius</returns>
        public static int ConvertTempData(string temperature)
        {
            var temperatureData = Convert.ToInt32(temperature, 16);
            return ConvertTempDataRange(temperatureData);
        }

        /// <summary>
        /// Converts time data to relative hex string
        /// </summary>
        /// <param name="timeData">
        /// Text based time data
        /// </param>
        /// <returns>Converted time data in byte</returns>
        public static byte ConvertTimeData(string timeData)
        {
            byte sum = 0x00;
            // TODO: Maybe able to improve this Ice machine time data conversion logic
            if (timeData.Contains('.') == false)
            {
                timeData += ".0";
            }

            // String number validation
            if (double.TryParse(timeData, out double _) && string.IsNullOrEmpty(timeData) == false)
            {
                var timeDataArr = timeData.Split('.');
                var secondsData = timeDataArr[0];
                var mSecondsData = timeDataArr[1];

                // Assign sensor values according to the bit position
                sum = secondsData switch
                {
                    "1" => 0x10,
                    "2" => 0x20,
                    "3" => 0x30,
                    "4" => 0x40,
                    "5" => 0x50,
                    "6" => 0x60,
                    "7" => 0x70,
                    "8" => 0x80,
                    "9" => 0x90,
                    "10" => 0xA0,
                    "11" => 0xB0,
                    "12" => 0xC0,
                    "13" => 0xD0,
                    "14" => 0xE0,
                    "15" => 0xF0,
                    _ => 0x00,
                };

                // Convert to byte so that it can be added to the byte data OUT stream
                sum += Convert.ToByte(mSecondsData);
                return sum;
            }
            else
            {
                return sum;
            }
        }

        /// <summary>
        /// Converts temperature data according to range (see Ice Machine protocol document for details)
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Converted Temperature data according to range specified in the Ice Machine protocol document</returns>
        private static int ConvertTempDataRange(int data)
        {
            var result = data switch
            {
                <= 127 => data,
                >= 128 => data - 256,
            };

            return result;
        }
}