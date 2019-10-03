namespace FlightCommander.SerialCommunication
{
    enum ReadControlWords
    {
        NONE = 0,
        IDENTIFY = 100,
        STATUS = 101,
        IMU = 102,
        SERVO = 103,
        MOTOR = 104,
        RC = 105,
        RAW_GPS = 106,
        GPS = 107,
        ATTITUDE = 108,
        ALTITUDE = 109,
        ANALOG = 110,
        RC_TUNING = 111,
        PID = 112,
        BOX = 113,
        MISC = 114,
        MOTOR_PINS = 115,
        BOX_NAME = 116,
        PID_NAME = 117,
        WAYPOINT = 118,
        BOX_ID = 119,
        SERVO_CONFIG = 120,
        NAV_STATUS = 121,
        NAV_CONFIG = 122,
        DEBUG = 252,
        DEBUG_STR = 253,
        DEBUG_VAL = 254,
        DROP_STATUS = 52,
        AIR_SPEED = 55,
    }

    enum WriteControlWords
    {
        NONE = 0,
        RETURN_TO_HOME = 50,
        DROP_BOMB = 51,
        RC = 200,
        GPS = 201,
        PID = 202,
        BOX = 203,
        RC_TUNING = 204,
        ACC_CALIBRATION = 205,
        MAG_CALIBRATION = 206,
        MISC = 207,
        RESET = 208,
        WAYPOINT = 209,
        SETTING_BANK = 210,
        HEAD = 211,
        SERVO_CONFIG = 212,
        MOTOR = 214,
        NAV_CONFIG = 215,
        BIND = 240,
        EEPROM_WRITE = 250,
    }

    class Protocol
    {
    }
}
