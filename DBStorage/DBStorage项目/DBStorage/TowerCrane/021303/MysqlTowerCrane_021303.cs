using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.Common;
using System.Text.RegularExpressions;
using SIXH.DBUtility;
using System.Threading;

namespace DBStorage._021303
{
    public class MysqlTowerCrane_021303
    {
        static Dictionary<DbHelperSQL, string> DbNetAndSn = new Dictionary<DbHelperSQL, string>();
        static MysqlTowerCrane_021303()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string[] connectionStringAry = connectionString.Split(';');
                foreach (string connectionStringTemp in connectionStringAry)
                {
                    string[] dbnetAry = connectionStringTemp.Split('&');
                    DbHelperSQL dbNet = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAry[0], dbnetAry[1], dbnetAry[2], dbnetAry[3], dbnetAry[4]), DbProviderType.MySql);
                    DbNetAndSn.Add(dbNet, "");
                }
                DbNetAndSnInit();
                Thread UpdateDbNetAndSnT = new Thread(UpdateDbNetAndSn) { IsBackground = true };
                UpdateDbNetAndSnT.Start();
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303异常", ex.Message);
            }
        }

        public static void TowerCraneDBFrameAnalyse(TowerCraneDBFrame dbf)
        {
            try
            {
                switch (dbf.datatype)
                {
                    case "heartbeat":
                        Heartbeat hb = Newtonsoft.Json.JsonConvert.DeserializeObject<Heartbeat>(dbf.contentjson);
                        Saveheartbeat(hb); break;
                    case "current":
                        Current cu = Newtonsoft.Json.JsonConvert.DeserializeObject<Current>(dbf.contentjson);
                        SaveCurrent(cu); break;
                    case "parameterUpload":
                        ParameterUpload pu = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload>(dbf.contentjson);
                        SaveParameterUpload(pu); break;
                    case "runtimeEp":
                        RuntimeEp re = Newtonsoft.Json.JsonConvert.DeserializeObject<RuntimeEp>(dbf.contentjson);
                        SaveRuntimeEp(re); break;
                    case "informationUpload":
                        InformationUpload iu = Newtonsoft.Json.JsonConvert.DeserializeObject<InformationUpload>(dbf.contentjson);
                        SaveInformationUpload(iu); break;
                    case "blackBox":
                        BlackBox bb = Newtonsoft.Json.JsonConvert.DeserializeObject<BlackBox>(dbf.contentjson);
                        SaveBlackBox(bb); break;
                    case "towerCraneBasicInformation":
                        TowerCraneBasicInformation tbi = Newtonsoft.Json.JsonConvert.DeserializeObject<TowerCraneBasicInformation>(dbf.contentjson);
                        SaveTowerCraneBasicInformation(tbi); break;
                    case "preventCollision":
                        PreventCollision pc = Newtonsoft.Json.JsonConvert.DeserializeObject<PreventCollision>(dbf.contentjson);
                        SavePreventCollision(pc); break;
                    case "localityProtection":
                        LocalityProtection lp = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalityProtection>(dbf.contentjson);
                        SaveLocalityProtection(lp); break;
                    default: break;
                }
                MysqlTowerCrane_Local.UpdateTowerCranedbtypeByid(dbf.id);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.TowerCraneDBFrameAnalyse异常", ex.Message);
            }
        }

        #region 心跳相关
        public static int Saveheartbeat(Heartbeat o)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(o.EquipmentID);
                if (DbNet != null)
                {
                    IList<DbParameter> paraList = new List<DbParameter>();
                    paraList.Add(DBoperateClass.DBoperateObj.CreateDbParameter("@craneNo", o.EquipmentID));
                    paraList.Add(DBoperateClass.DBoperateObj.CreateDbParameter("@onlineTimes", o.OnlineTime));
                    int y = DbNet.ExecuteNonQuery("pro_heartbeat", paraList, CommandType.StoredProcedure);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("心跳", o.EquipmentID + ";" + y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.Saveheartbeat异常", ex.Message);
                return 0;
            }
        }
        #endregion

        #region 实时数据相关
        public static int SaveCurrent(Current current)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(current.EquipmentID);
                if (DbNet != null)
                {
                    IList<DbParameter> paraList = new List<DbParameter>();
                    paraList.Add(DbNet.CreateDbParameter("@craneNo", current.EquipmentID));
                    paraList.Add(DbNet.CreateDbParameter("@ctimes", current.RTC));
                    paraList.Add(DbNet.CreateDbParameter("@workCircle", current.WorkCircle));
                    string card = current.DriverCardNo.Replace("u0000", "");
                    if (card.Length > 20)
                        card = card.Substring(0, 20);
                    paraList.Add(DbNet.CreateDbParameter("@cardNo", current.DriverCardNo.Replace("u0000", "")));
                    paraList.Add(DbNet.CreateDbParameter("@rtc", current.RTC));
                    paraList.Add(DbNet.CreateDbParameter("@powerStatus", current.PowerState));
                    paraList.Add(DbNet.CreateDbParameter("@times", current.Times));
                    paraList.Add(DbNet.CreateDbParameter("@height", StringByDouble.ConvertDoubleString(current.Height, 100)));
                    paraList.Add(DbNet.CreateDbParameter("@radius", StringByDouble.ConvertDoubleString(current.Range, 100)));
                    paraList.Add(DbNet.CreateDbParameter("@angle", StringByDouble.ConvertDoubleString(current.Rotation, 10)));
                    paraList.Add(DbNet.CreateDbParameter("@weight", StringByDouble.ConvertDoubleString(current.Weight, 100)));
                    paraList.Add(DbNet.CreateDbParameter("@wind", StringByDouble.ConvertDoubleString(current.WindSpeed, 100)));
                    paraList.Add(DbNet.CreateDbParameter("@angleX", StringByDouble.ConvertDoubleString(current.DipAngle_X, 100)));
                    paraList.Add(DbNet.CreateDbParameter("@angleY", StringByDouble.ConvertDoubleString(current.DipAngle_Y, 100)));
                    paraList.Add(DbNet.CreateDbParameter("@armAngle", StringByDouble.ConvertDoubleString(current.BoomAngle, 100)));
                    paraList.Add(DbNet.CreateDbParameter("@distance", StringByDouble.ConvertDoubleString(current.Stroke, 10)));
                    paraList.Add(DbNet.CreateDbParameter("@safeRadius", StringByDouble.ConvertDoubleString(current.SafeRange, 100)));
                    paraList.Add(DbNet.CreateDbParameter("@safeWeight", StringByDouble.ConvertDoubleString(current.SafeWeight, 100)));
                    paraList.Add(DbNet.CreateDbParameter("@safeTorque", StringByDouble.ConvertDoubleString(current.SafeMoment, 10)));
                    //力矩百分比
                    double MomentPercent = 0d;
                    if (current.SafeMoment / 10 != 0)
                        MomentPercent = ((double)((double)current.Weight / 100d) * (double)((double)current.Range / 100d) / (double)((double)current.SafeMoment / 10d));
                    else
                        MomentPercent = 0d;
                    paraList.Add(DbNet.CreateDbParameter("@torquePercent", MomentPercent));
                    paraList.Add(DbNet.CreateDbParameter("@hitChannelStatus", current.CollisionCommunicationState));
                    paraList.Add(DbNet.CreateDbParameter("@modelStatus", current.ModuleState));
                    paraList.Add(DbNet.CreateDbParameter("@relayStatus", current.RelayState));
                    paraList.Add(DbNet.CreateDbParameter("@sensorStatus", current.SensorState));
                    paraList.Add(DbNet.CreateDbParameter("@warnType", current.WarningMessage));
                    paraList.Add(DbNet.CreateDbParameter("@alarmType", current.AlarmMessage));
                    paraList.Add(DbNet.CreateDbParameter("@craneScreen", current.EquipmentDisplaying));
                    paraList.Add(DbNet.CreateDbParameter("@setInfo", current.CanSetMessage));
                    //ht.Add("windLevel", ConvertWind.WindToLeve(current.WindSpeed / 100.0f));
                    #region 副钩信息
                    paraList.Add(DbNet.CreateDbParameter("@viceArmNum", current.ViceHookCount));
                    string vh_times = "", vh_height = "", vh_radius = "", vh_weight = "", vh_safeRadius = "", vh_safeWeight = "", vh_safeTorque = "", vh_relayStatus = "", vh_sensorStatus = "", vh_warnType = "", vh_alarmType = "";
                    try
                    {
                        foreach (Current.ViceHook cvsp in current.ViceHookMessage)
                        {
                            vh_times += cvsp.Times.ToString() + "&";
                            vh_height += cvsp.Height.ToString() + "&";
                            vh_radius += cvsp.Range.ToString() + "&";
                            vh_weight += cvsp.Weight.ToString() + "&";
                            vh_safeRadius += cvsp.SafeRange.ToString() + "&";
                            vh_safeWeight += cvsp.SafeWeight.ToString() + "&";
                            vh_safeTorque += cvsp.SafeMoment.ToString() + "&";
                            vh_relayStatus += cvsp.RelayState.ToString() + "&";
                            vh_sensorStatus += cvsp.SensorState.ToString() + "&";
                            vh_warnType += cvsp.WarningMessage.ToString() + "&";
                            vh_alarmType += cvsp.AlarmMessage.ToString() + "&";
                        }
                    }
                    catch (Exception) { }
                    paraList.Add(DbNet.CreateDbParameter("@vh_times", SQLJoint.RemoveLastChar(vh_times)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_height", SQLJoint.RemoveLastChar(vh_height)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_radius", SQLJoint.RemoveLastChar(vh_radius)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_weight", SQLJoint.RemoveLastChar(vh_weight)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_safeRadius", SQLJoint.RemoveLastChar(vh_safeRadius)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_safeWeight", SQLJoint.RemoveLastChar(vh_safeWeight)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_safeTorque", SQLJoint.RemoveLastChar(vh_safeTorque)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_relayStatus", SQLJoint.RemoveLastChar(vh_relayStatus)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_sensorStatus", SQLJoint.RemoveLastChar(vh_sensorStatus)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_warnType", SQLJoint.RemoveLastChar(vh_warnType)));
                    paraList.Add(DbNet.CreateDbParameter("@vh_alarmType", SQLJoint.RemoveLastChar(vh_alarmType)));
                    #endregion
                    int y = DbNet.ExecuteNonQuery("pro_realData_v2_13_New", paraList, CommandType.StoredProcedure);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("实时数据", current.EquipmentID + ";" + y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.SaveCurrent异常", ex.Message);
                return 0;
            }
        }
        #endregion

        #region 参数上传相关
        public static int SaveParameterUpload(ParameterUpload o)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(o.EquipmentID);
                if (DbNet != null)
                {
                    #region 拼接SQL
                    string text = "INSERT INTO t_craneconfig_tower_v2_13 (", value = ") VALUES ( ";
                    SQLJoint.AddField("craneNo", o.EquipmentID, true, ref text, ref value);
                    SQLJoint.AddField("RTC", o.ChangeRTC, true, ref text, ref value);
                    SQLJoint.AddField("updateMark", o.ModifyIdentification, true, ref text, ref value);
                    #region 高度设置
                    string sss = o.HeightSet.ToString();
                    string SensorType = GetAttributeValue(sss, "SensorType").Trim();
                    ParameterUpload.AHeightSet ahs;
                    if (SensorType.Equals("0"))
                    {
                        ParameterUpload.HeightSet_Potentiometer pa = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.HeightSet_Potentiometer>(o.HeightSet.ToString().Replace("\r\n", ""));
                        ahs = pa;
                    }
                    else
                    {
                        ParameterUpload.HeightSet_Coder ca = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.HeightSet_Coder>(o.HeightSet.ToString().Replace("\r\n", ""));
                        SQLJoint.AddField("high_increaseDirectionValue", ca.SamplingAddDirection, true, ref text, ref value);
                        SQLJoint.AddField("high_initCircleNum", ca.InitialCylinderNumber, false, ref text, ref value);
                        ahs = ca;
                    }
                    SQLJoint.AddField("high_sensorType", ahs.SensorType, true, ref text, ref value);
                    SQLJoint.AddField("high_upLimitValue", ahs.UpperLimitSampling, false, ref text, ref value);
                    SQLJoint.AddField("high_downLimitValue", ahs.LowerLimitSampling, false, ref text, ref value);
                    SQLJoint.AddField("high_towerHeight", ahs.TowerBodyheight, false, ref text, ref value);
                    SQLJoint.AddField("high_reductionValue", ahs.ReductionSpeed, false, ref text, ref value);
                    SQLJoint.AddField("high_restrictValue", ahs.Limit, false, ref text, ref value);
                    SQLJoint.AddField("high_topRiseIdentifies", ahs.JackingMark, true, ref text, ref value);
                    #endregion
                    #region 幅度设置
                    sss = o.RangeSet.ToString();
                    SensorType = GetAttributeValue(sss, "SensorType").Trim();
                    ParameterUpload.ARangeSet ars;
                    if (SensorType.Equals("0"))
                    {
                        ParameterUpload.RangeSet_Potentiometer pa = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.RangeSet_Potentiometer>(o.RangeSet.ToString().Replace("\r\n", ""));
                        ars = pa;
                    }
                    else
                    {
                        ParameterUpload.RangeSet_Coder rc = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.RangeSet_Coder>(o.RangeSet.ToString().Replace("\r\n", ""));
                        SQLJoint.AddField("amp_increaseDirectionValue", rc.SamplingAddDirection, true, ref text, ref value);
                        SQLJoint.AddField("amp_initCircleNum", rc.InitialCylinderNumber, false, ref text, ref value);
                        ars = rc;
                    }
                    SQLJoint.AddField("amp_sensorType", ars.SensorType, true, ref text, ref value);
                    SQLJoint.AddField("amp_insideLimitValue", ars.InsideLimitSampling, false, ref text, ref value);
                    SQLJoint.AddField("amp_outsideLmitValue", ars.OutsideLimitSampling, false, ref text, ref value);
                    SQLJoint.AddField("minAmplitude", ars.MinARange, false, ref text, ref value);
                    SQLJoint.AddField("maxAmplitude", ars.MaxARange, false, ref text, ref value);
                    SQLJoint.AddField("ampReductionValue", ars.ReductionSpeed, false, ref text, ref value);
                    SQLJoint.AddField("ampRestrictValue", ars.Limit, false, ref text, ref value);
                    #endregion
                    #region 回转设置
                    sss = o.RotationSet.ToString();
                    SensorType = GetAttributeValueComma(sss, "RotationType").Trim();
                    if (SensorType == "1")
                    {
                        ParameterUpload.RotationSet_A temp = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.RotationSet_A>(o.RotationSet.ToString().Replace("\r\n", ""));
                        SQLJoint.AddField("rotaryType", temp.RotationType, true, ref text, ref value);
                        SQLJoint.AddField("turn_leftLimit", temp.LeftLimitSampling, false, ref text, ref value);
                        SQLJoint.AddField("trun_rightLimit", temp.RightLimitSampling, false, ref text, ref value);
                        SQLJoint.AddField("turn_leftRightLimit", temp.EquipmentIDLeftAndRightLimitAngleSum, false, ref text, ref value);
                        SQLJoint.AddField("turn_reducionValue", temp.ReductionSpeed, false, ref text, ref value);
                        SQLJoint.AddField("turn_restrictValue", temp.Limit, false, ref text, ref value);
                    }
                    else if (SensorType == "2")
                    {
                        ParameterUpload.RotationSet_B temp = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.RotationSet_B>(o.RotationSet.ToString().Replace("\r\n", ""));
                        SQLJoint.AddField("rotaryType", temp.RotationType, true, ref text, ref value);
                        SQLJoint.AddField("turn_setDirection", temp.Direction, true, ref text, ref value);
                        SQLJoint.AddField("turn_dirButtonValue", temp.DirectionButtonGather, false, ref text, ref value);
                        SQLJoint.AddField("turn_conButtonValue", temp.ConfirmButtonGather, false, ref text, ref value);
                    }
                    else if (SensorType == "3")
                    {
                        ParameterUpload.RotationSet_C temp = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.RotationSet_C>(o.RotationSet.ToString().Replace("\r\n", ""));
                        SQLJoint.AddField("rotaryType", temp.RotationType, true, ref text, ref value);
                        SQLJoint.AddField("turn_zeroDegree", temp.ZeroAngleSampling, true, ref text, ref value);
                        SQLJoint.AddField("turn_setAngleValue", temp.SetAngleSampling, false, ref text, ref value);
                        SQLJoint.AddField("turn_setAngle", temp.SetAngle, false, ref text, ref value);
                        SQLJoint.AddField("turn_reducionValue", temp.ReductionSpeed, false, ref text, ref value);
                        SQLJoint.AddField("turn_restrictValue", temp.Limit, false, ref text, ref value);
                    }
                    else if (SensorType == "4")
                    {
                        ParameterUpload.RotationSet_D temp = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.RotationSet_D>(o.RotationSet.ToString().Replace("\r\n", ""));
                        SQLJoint.AddField("rotaryType", temp.RotationType, true, ref text, ref value);
                        SQLJoint.AddField("turn_relZeroDegree", temp.RelativeAbsoluteZeroIdentification, true, ref text, ref value);
                        SQLJoint.AddField("turn_rotateDirection", temp.RotateDirection, true, ref text, ref value);
                    }
                    else if (SensorType == "5")
                    {
                        ParameterUpload.RotationSet_E temp = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.RotationSet_E>(o.RotationSet.ToString().Replace("\r\n", ""));
                        SQLJoint.AddField("rotaryType", temp.RotationType, true, ref text, ref value);
                        SQLJoint.AddField("turn_increaseDirectionValue", temp.SamplingAddDirection, true, ref text, ref value);
                        SQLJoint.AddField("turn_initCircleNum", temp.InitialCylinderNumber, false, ref text, ref value);
                        SQLJoint.AddField("turn_sensorTransRatio", temp.SensorDriveRatio[0].ToString() + "&" + temp.SensorDriveRatio[1].ToString(), true, ref text, ref value);
                        SQLJoint.AddField("turn_towerTeethNum", temp.TowerTeeth, false, ref text, ref value);
                        SQLJoint.AddField("turn_sensorTeethNum", temp.SensorTeeth, false, ref text, ref value);
                        SQLJoint.AddField("turn_zeroSensorValue", temp.ZeroSensor, false, ref text, ref value);
                        SQLJoint.AddField("turn_inverseSensorValue", temp.anticlockwiseSensor, false, ref text, ref value);
                    }
                    #endregion
                    #region 重量设置
                    ParameterUpload.WeightSet_AnalogSignal aws =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.WeightSet_AnalogSignal>(o.WeightSet.ToString().Replace("\r\n", ""));
                    SQLJoint.AddField("weight_sensorType", aws.SensorType, true, ref text, ref value);
                    SQLJoint.AddField("weight_setRatio", aws.SetRate, false, ref text, ref value);
                    SQLJoint.AddField("weight_emptyhookAD", aws.EmptyHookSampling, false, ref text, ref value);
                    SQLJoint.AddField("weight_loadWeightAD", aws.CraneWeightSampling, false, ref text, ref value);
                    SQLJoint.AddField("weight_farmarWeight", aws.WeightWeight, false, ref text, ref value);
                    SQLJoint.AddField("weight_throwOverWeight", aws.ThrowOverWeight, false, ref text, ref value);
                    SQLJoint.AddField("weight_cutWeight", aws.CutOffWeight, false, ref text, ref value);
                    #endregion
                    #region 力矩设置
                    ParameterUpload.CMomentSet cms = o.MomentSet as ParameterUpload.CMomentSet;
                    SQLJoint.AddField("torque_throwOverTorque", cms.ThrowOverMomentSet, false, ref text, ref value);
                    SQLJoint.AddField("torque_cutTorque", cms.CutOffMomentSet, false, ref text, ref value);
                    #endregion
                    #region 风速设置
                    ParameterUpload.CWindSpeedSet cws = o.WindSpeedSet as ParameterUpload.CWindSpeedSet;
                    SQLJoint.AddField("windType", cws.WindSpeedType, true, ref text, ref value);
                    SQLJoint.AddField("windUnit", cws.WindSpeedUnit, true, ref text, ref value);
                    SQLJoint.AddField("windWarn", cws.WindSpeedWarning, false, ref text, ref value);
                    SQLJoint.AddField("windAlarm", cws.WindSpeedAlarm, false, ref text, ref value);
                    #endregion
                    #region 倾角设置
                    ParameterUpload.CDipAngleSet cds = o.DipAngleSet as ParameterUpload.CDipAngleSet;
                    SQLJoint.AddField("angleType", cds.DipAngleType, false, ref text, ref value);
                    SQLJoint.AddField("angleRelativeZeroMark", cds.DipAngleRelativelyZeroFlag, false, ref text, ref value);
                    SQLJoint.AddField("angleWarn", cds.DipAngleWarning, false, ref text, ref value);
                    SQLJoint.AddField("angleAlarm", cds.DipAngleAlarm, false, ref text, ref value);
                    #endregion
                    #region 仰角设置
                    ParameterUpload.CBoomAngleSet cbs = o.BoomAngleSet as ParameterUpload.CBoomAngleSet;
                    SQLJoint.AddField("elevationSensorType", cbs.BoomAngleType, false, ref text, ref value);
                    SQLJoint.AddField("elevationRelativeZeroMark", cbs.BoomAngleRelativelyZeroFlag, false, ref text, ref value);
                    SQLJoint.AddField("minElevationAngleValue", cbs.BoomAngleMin, false, ref text, ref value);
                    SQLJoint.AddField("maxMovableArmElevationValue", cbs.BoomAngleMax, false, ref text, ref value);
                    SQLJoint.AddField("elevationAngleReductionValue", cbs.BoomAngleThrowOver, false, ref text, ref value);
                    SQLJoint.AddField("elevationAngleRestrictValue", cbs.BoomAngleLimit, false, ref text, ref value);
                    #endregion
                    SQLJoint.AddField("towerEyeNum", o.CraneEyeNumber, true, ref text, ref value);
                    #region 塔吊眼参数
                    string HorizontalAngleStr = "", AngleOfPitchStr = "", SpeedDomeCamerasMagnification = "", TargetAltitudeStr = "", FocusingFactorStr = "", LongArmStr = "", AlgorithmStr = "", FocusingMin = "", FocusingMax = "";
                    try
                    {
                        foreach (ParameterUpload.CCraneEyeParameter ccep in o.CraneEyeParameter)
                        {
                            HorizontalAngleStr += ccep.HorizontalAngle.ToString() + "&";
                            AngleOfPitchStr += ccep.AngleOfPitch.ToString() + "&";
                            SpeedDomeCamerasMagnification += ccep.SpeedDomeCamerasMagnification.ToString() + "&";
                            TargetAltitudeStr += ccep.TargetAltitude.ToString() + "&";
                            FocusingFactorStr += ccep.FocusingFactor.ToString() + "&";
                            LongArmStr += ccep.LongArm.ToString() + "&";
                            AlgorithmStr += ccep.Algorithm.ToString() + "&";
                            FocusingMin += ccep.FocusingMin.ToString() + "&";
                            FocusingMax += ccep.FocusingMax.ToString() + "&";
                        }
                    }
                    catch (Exception) { }
                    SQLJoint.AddField("teye_horizontalAngle", SQLJoint.RemoveLastChar(HorizontalAngleStr), true, ref text, ref value);
                    SQLJoint.AddField("teye_pitchingAngle", SQLJoint.RemoveLastChar(AngleOfPitchStr), true, ref text, ref value);
                    SQLJoint.AddField("teye_magnificationTimes", SQLJoint.RemoveLastChar(SpeedDomeCamerasMagnification), true, ref text, ref value);
                    SQLJoint.AddField("teye_targetHeight", SQLJoint.RemoveLastChar(TargetAltitudeStr), true, ref text, ref value);
                    SQLJoint.AddField("teye_focusCoefficient", SQLJoint.RemoveLastChar(FocusingFactorStr), true, ref text, ref value);
                    SQLJoint.AddField("teye_bigArmLength", SQLJoint.RemoveLastChar(LongArmStr), true, ref text, ref value);
                    SQLJoint.AddField("teye_algorithm", SQLJoint.RemoveLastChar(AlgorithmStr), true, ref text, ref value);
                    SQLJoint.AddField("teye_focusMinValue", SQLJoint.RemoveLastChar(FocusingMin), true, ref text, ref value);
                    SQLJoint.AddField("teye_focusMaxValue", SQLJoint.RemoveLastChar(FocusingMax), true, ref text, ref value);
                    #endregion
                    SQLJoint.AddField("viceArmNum", o.ViceHookCount, true, ref text, ref value);
                    #region 副臂参数
                    string vh_high_sensorType = "", vh_high_increaseDirectionValue = "", vh_high_initCircleNum = "", vh_high_upLimitValue = "", vh_high_downLimitValue = "", vh_high_towerHeight = "", vh_high_reductionValue = "", vh_high_restrictValue = "", vh_high_topRiseIdentifies = "", vh_weight_sensorType = "",
                        vh_setRatio = "", vh_emptyhookAD = "", vh_loadWeightAD = "", vh_farmarWeight = "", vh_throwOverWeight = "", vh_cutWeight = "", vh_throwOverTorque = "", vh_cutTorque = "";
                    try
                    {
                        foreach (ParameterUpload.CViceHookSetParameter cvsp in o.ViceHookSetParameter)
                        {
                            #region 高度设置
                            sss = cvsp.HeightSet.ToString();
                            SensorType = GetAttributeValue(sss, "SensorType").Trim();
                            ParameterUpload.AHeightSet ahsV;
                            if (SensorType.Equals("0"))
                            {
                                ParameterUpload.HeightSet_Potentiometer pa = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.HeightSet_Potentiometer>(cvsp.HeightSet.ToString().Replace("\r\n", ""));
                                ahsV = pa;
                            }
                            else
                            {
                                ParameterUpload.HeightSet_Coder hc = Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.HeightSet_Coder>(cvsp.HeightSet.ToString().Replace("\r\n", ""));
                                vh_high_increaseDirectionValue += hc.SamplingAddDirection.ToString() + "&";
                                vh_high_initCircleNum += hc.InitialCylinderNumber.ToString() + "&";
                                ahsV = hc;
                            }
                            vh_high_sensorType += ahsV.SensorType.ToString() + "&";
                            vh_high_upLimitValue += ahsV.UpperLimitSampling.ToString() + "&";
                            vh_high_downLimitValue += ahsV.LowerLimitSampling.ToString() + "&";
                            vh_high_towerHeight += ahsV.TowerBodyheight.ToString() + "&";
                            vh_high_reductionValue += ahsV.ReductionSpeed.ToString() + "&";
                            vh_high_restrictValue += ahsV.Limit.ToString() + "&";
                            vh_high_topRiseIdentifies += ahsV.JackingMark.ToString() + "&";
                            #endregion
                            #region 重量设置
                            ParameterUpload.WeightSet_AnalogSignal awsV =
                            Newtonsoft.Json.JsonConvert.DeserializeObject<ParameterUpload.WeightSet_AnalogSignal>(cvsp.WeightSet.ToString().Replace("\r\n", ""));
                            vh_weight_sensorType += awsV.SensorType.ToString() + "&";
                            vh_setRatio += awsV.SetRate.ToString() + "&";
                            vh_emptyhookAD += awsV.EmptyHookSampling.ToString() + "&";
                            vh_loadWeightAD += awsV.CraneWeightSampling.ToString() + "&";
                            vh_farmarWeight += awsV.WeightWeight.ToString() + "&";
                            vh_throwOverWeight += awsV.ThrowOverWeight.ToString() + "&";
                            vh_cutWeight += awsV.CutOffWeight.ToString() + "&";
                            #endregion
                            #region 力矩设置
                            ParameterUpload.CMomentSet cmsV = cvsp.MomentSet as ParameterUpload.CMomentSet;
                            vh_throwOverTorque += cmsV.ThrowOverMomentSet.ToString() + "&";
                            vh_cutTorque += cmsV.CutOffMomentSet.ToString() + "&";
                            #endregion
                        }
                    }
                    catch (Exception) { }
                    SQLJoint.AddField("vh_high_sensorType", SQLJoint.RemoveLastChar(vh_high_sensorType), true, ref text, ref value);
                    SQLJoint.AddField("vh_high_increaseDirectionValue", SQLJoint.RemoveLastChar(vh_high_increaseDirectionValue), true, ref text, ref value);
                    SQLJoint.AddField("vh_high_initCircleNum", SQLJoint.RemoveLastChar(vh_high_initCircleNum), true, ref text, ref value);
                    SQLJoint.AddField("vh_high_upLimitValue", SQLJoint.RemoveLastChar(vh_high_upLimitValue), true, ref text, ref value);
                    SQLJoint.AddField("vh_high_downLimitValue", SQLJoint.RemoveLastChar(vh_high_downLimitValue), true, ref text, ref value);
                    SQLJoint.AddField("vh_high_towerHeight", SQLJoint.RemoveLastChar(vh_high_towerHeight), true, ref text, ref value);
                    SQLJoint.AddField("vh_high_reductionValue", SQLJoint.RemoveLastChar(vh_high_reductionValue), true, ref text, ref value);
                    SQLJoint.AddField("vh_high_restrictValue", SQLJoint.RemoveLastChar(vh_high_restrictValue), true, ref text, ref value);
                    SQLJoint.AddField("vh_high_topRiseIdentifies", SQLJoint.RemoveLastChar(vh_high_topRiseIdentifies), true, ref text, ref value);
                    SQLJoint.AddField("vh_weight_sensorType", SQLJoint.RemoveLastChar(vh_weight_sensorType), true, ref text, ref value);
                    SQLJoint.AddField("vh_setRatio", SQLJoint.RemoveLastChar(vh_setRatio), true, ref text, ref value);
                    SQLJoint.AddField("vh_emptyhookAD", SQLJoint.RemoveLastChar(vh_emptyhookAD), true, ref text, ref value);
                    SQLJoint.AddField("vh_loadWeightAD", SQLJoint.RemoveLastChar(vh_loadWeightAD), true, ref text, ref value);
                    SQLJoint.AddField("vh_farmarWeight", SQLJoint.RemoveLastChar(vh_farmarWeight), true, ref text, ref value);
                    SQLJoint.AddField("vh_throwOverWeight", SQLJoint.RemoveLastChar(vh_throwOverWeight), true, ref text, ref value);
                    SQLJoint.AddField("vh_cutWeight", SQLJoint.RemoveLastChar(vh_cutWeight), true, ref text, ref value);
                    SQLJoint.AddField("vh_throwOverTorque", SQLJoint.RemoveLastChar(vh_throwOverTorque), true, ref text, ref value);
                    SQLJoint.AddField("vh_cutTorque", SQLJoint.RemoveLastChar(vh_cutTorque), true, ref text, ref value);
                    #endregion
                    SQLJoint.AddField("updateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true, ref text, ref value);

                    text = SQLJoint.RemoveLastChar(text);
                    value = SQLJoint.RemoveLastChar(value);
                    string resultSql = text + value + ")";
                    #endregion
                    int y = DbNet.ExecuteNonQuery(resultSql, null, CommandType.Text);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("参数上传", o.EquipmentID + ";" + y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.SaveParameterUpload异常", ex.Message + ex.StackTrace);
                return 0;
            }
        }
        #endregion

        #region 设备运行时间
        public static int SaveRuntimeEp(RuntimeEp o)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(o.EquipmentID);
                if (DbNet != null)
                {
                    string sql = String.Format(" update  t_cranecurrent_tower set runTime='{0}',sumRunTime='{1}' where craneNo='{2}'", o.StartingUpRuntime, o.TotalRuntime, o.EquipmentID);
                    int y = DbNet.ExecuteNonQuery(sql, null, CommandType.Text);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("运行时间", o.EquipmentID + ";" + y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.SaveRuntimeEp异常", ex.Message + ex.StackTrace);
                return 0;
            }
        }
        #endregion

        #region 信息上传
        public static int SaveInformationUpload(InformationUpload o)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(o.EquipmentID);
                if (DbNet != null)
                {
                    #region 拼接SQL
                    string text = "INSERT INTO t_infoUpload_tower_v2_13 (", value = ") VALUES ( ";
                    SQLJoint.AddField("craneNo", o.EquipmentID, true, ref text, ref value);
                    SQLJoint.AddField("infoRTC", o.RTC, true, ref text, ref value);
                    SQLJoint.AddField("infoType", o.InformationType, true, ref text, ref value);
                    SQLJoint.AddField("infoUploadCode", o.InformationCode, true, ref text, ref value);
                    SQLJoint.AddField("updateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true, ref text, ref value);
                    text = SQLJoint.RemoveLastChar(text);
                    value = SQLJoint.RemoveLastChar(value);
                    string resultSql = text + value + ")";
                    #endregion
                    int y = DbNet.ExecuteNonQuery(resultSql, null, CommandType.Text);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("信息上传", o.EquipmentID + ";" + y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.SaveInformationUpload异常", ex.Message + ex.StackTrace);
                return 0;
            }
        }
        #endregion

        #region 黑匣子
        public static int SaveBlackBox(BlackBox o)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(o.EquipmentID);
                if (DbNet != null)
                {
                    #region 拼接SQL  插入黑匣子
                    string text = "INSERT INTO t_crane_v2_13 (", value = ") VALUES ( ";
                    SQLJoint.AddField("craneNo", o.EquipmentID, true, ref text, ref value);
                    SQLJoint.AddField("functionLockIdentifies", o.FunctionLockIdentifier, true, ref text, ref value);
                    SQLJoint.AddField("functionConfig", o.FunctionConfiguration, true, ref text, ref value);
                    #region 经纬度
                    BlackBox.CLongitudeAndLatitude cll = o.LongitudeAndLatitude as BlackBox.CLongitudeAndLatitude;
                    SQLJoint.AddField("geoCoordinateX", cll.Latitude, true, ref text, ref value);
                    SQLJoint.AddField("geoCoordinateY", cll.Longitude, true, ref text, ref value);
                    SQLJoint.AddField("northOrSouthX", cll.LatitudeType, true, ref text, ref value);
                    SQLJoint.AddField("eastOrWestY", cll.LongitudeType, true, ref text, ref value);
                    #endregion
                    SQLJoint.AddField("identificationWayLock", o.IdentificationTypeLockIdentifier, true, ref text, ref value);
                    SQLJoint.AddField("identificationWay", o.IdentificationType, true, ref text, ref value);
                    SQLJoint.AddField("identificationCycleOnOff", o.IdentificationCycleSwitchState, true, ref text, ref value);
                    SQLJoint.AddField("identificationCycle", o.IdentificationCycle, true, ref text, ref value);
                    SQLJoint.AddField("brightnessSettings", o.BrightnessSetting[0].ToString() + "&" + o.BrightnessSetting[1].ToString(), true, ref text, ref value);
                    SQLJoint.AddField("zigbeeVersion", o.Version_Zigbee, true, ref text, ref value);
                    SQLJoint.AddField("relayVersion", o.Version_Relay, true, ref text, ref value);
                    SQLJoint.AddField("fromMachineVersion", o.Version_Counterpart, true, ref text, ref value);
                    SQLJoint.AddField("identificationVersion", o.Version_IDCard, true, ref text, ref value);
                    SQLJoint.AddField("softwareVersion", o.Version_Software, true, ref text, ref value);

                    SQLJoint.AddField("updateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true, ref text, ref value);
                    text = SQLJoint.RemoveLastChar(text);
                    value = SQLJoint.RemoveLastChar(value);
                    string resultSql = text + value + ")";
                    #endregion
                    int y = DbNet.ExecuteNonQuery(resultSql, null, CommandType.Text);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("黑匣子", o.EquipmentID + ";" + y.ToString());
                    #region 更新主表的设备版本号
                    string updataT_crane = string.Format("update t_crane set softwareVersion='v2.13.3' WHERE craneNo='{0}'", o.EquipmentID);
                    DbNet.ExecuteNonQuery(updataT_crane, null, CommandType.Text);
                    #endregion
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.SaveBlackBox异常", ex.Message + ex.StackTrace);
                return 0;
            }
        }
        #endregion

        #region 塔吊基本信息
        public static int SaveTowerCraneBasicInformation(TowerCraneBasicInformation o)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(o.EquipmentID);
                if (DbNet != null)
                {
                    #region 拼接SQL
                    string text = "INSERT INTO t_crane_tower_v2_13 (", value = ") VALUES ( ";
                    SQLJoint.AddField("craneNo", o.EquipmentID, true, ref text, ref value);
                    SQLJoint.AddField("towerType", o.TowerCraneType, true, ref text, ref value);
                    SQLJoint.AddField("armFor", o.BoomArmLength, false, ref text, ref value);
                    SQLJoint.AddField("armBak", o.BalanceArmLength, false, ref text, ref value);
                    SQLJoint.AddField("rootHeight", o.TowerBodyHeight, false, ref text, ref value);
                    SQLJoint.AddField("headHeight", o.towerCapORElevation, false, ref text, ref value);
                    SQLJoint.AddField("minElevationAngle", o.MinBoomAngle, false, ref text, ref value);

                    SQLJoint.AddField("torqueType", o.MomentCurveType, true, ref text, ref value);
                    SQLJoint.AddField("torqueCurveCount", o.MomentCurveCount, true, ref text, ref value);
                    #region 力矩曲线类型
                    if (o.MomentCurveType == 1)
                    {
                        string ratioValue = "", maxLiftWeight = "", maxLiftWeightRange = "", maxRange = "", maxRangeLiftWeight = "";
                        try
                        {
                            foreach (object obj in o.MomentCurveSet)
                            {
                                TowerCraneBasicInformation.MomentCurveSet_Curve mcsc =
                                Newtonsoft.Json.JsonConvert.DeserializeObject<TowerCraneBasicInformation.MomentCurveSet_Curve>(obj.ToString().Replace("\r\n", ""));
                                ratioValue += mcsc.Rate.ToString() + "&";
                                maxLiftWeight += mcsc.MaxWeight.ToString() + "&";
                                maxLiftWeightRange += mcsc.MaxWeightRange.ToString() + "&";
                                maxRange += mcsc.MaxRange.ToString() + "&";
                                maxRangeLiftWeight += mcsc.MaxRangeWeight.ToString() + "&";
                            }
                        }
                        catch (Exception) { }
                        SQLJoint.AddField("ratioValue", SQLJoint.RemoveLastChar(ratioValue), true, ref text, ref value);
                        SQLJoint.AddField("maxLiftWeight", SQLJoint.RemoveLastChar(maxLiftWeight), true, ref text, ref value);
                        SQLJoint.AddField("maxLiftWeightRange", SQLJoint.RemoveLastChar(maxLiftWeightRange), true, ref text, ref value);
                        SQLJoint.AddField("maxRange", SQLJoint.RemoveLastChar(maxRange), true, ref text, ref value);
                        SQLJoint.AddField("maxRangeLiftWeight", SQLJoint.RemoveLastChar(maxRangeLiftWeight), true, ref text, ref value);
                    }
                    else if (o.MomentCurveType == 0)
                    {
                        string ratioValue = "", curveSetPointNum = "", nPointWeight = "", nPointWeightRange = "";
                        try
                        {
                            foreach (object obj in o.MomentCurveSet)
                            {
                                TowerCraneBasicInformation.MomentCurveSet_Icon mcsi =
                                Newtonsoft.Json.JsonConvert.DeserializeObject<TowerCraneBasicInformation.MomentCurveSet_Icon>(obj.ToString().Replace("\r\n", ""));
                                ratioValue += mcsi.Rate.ToString() + "&";
                                curveSetPointNum += mcsi.CurveSetPointCount.ToString() + "&";
                                foreach (TowerCraneBasicInformation.MomentCurveSet_Icon.MomentCurve_Icon icTemp in mcsi.MomentCurve_IconAry)
                                {
                                    nPointWeight += icTemp.Weight.ToString() + "&";
                                    nPointWeightRange += icTemp.WeightRange.ToString() + "&";
                                }
                                nPointWeight = SQLJoint.RemoveLastChar(nPointWeight) + "#";
                                nPointWeightRange = SQLJoint.RemoveLastChar(nPointWeightRange) + "#";
                            }
                        }
                        catch (Exception) { }
                        SQLJoint.AddField("ratioValue", SQLJoint.RemoveLastChar(ratioValue), true, ref text, ref value);
                        SQLJoint.AddField("curveSetPointNum", SQLJoint.RemoveLastChar(curveSetPointNum), true, ref text, ref value);
                        SQLJoint.AddField("nPointWeight", SQLJoint.RemoveLastChar(nPointWeight), true, ref text, ref value);
                        SQLJoint.AddField("nPointWeightRange", SQLJoint.RemoveLastChar(nPointWeightRange), true, ref text, ref value);
                    }
                    #endregion


                    SQLJoint.AddField("viceArmNum", o.ViceHookCount, true, ref text, ref value);
                    #region 副钩类
                    string viceArmLength = "", viceArmAngle = "", viceTorqueType = ""
                        , viceTorqueCurveCount = "";
                    string vice_ratioValue = "", vice_maxLiftWeight = "", vice_maxLiftWeightRange = "", vice_maxRange = "", vice_maxRangeLiftWeight = "", vice_curveSetPointNum = "", vice_nPointWeight = "", vice_nPointWeightRange = "";
                    try
                    {
                        foreach (TowerCraneBasicInformation.ViceHook vh in o.ViceHookMessage)
                        {
                            viceArmLength += vh.ViceArmLength.ToString() + "&";
                            viceArmAngle += vh.ViceArmIntersectionAngle.ToString() + "&";
                            viceTorqueType += vh.MomentCurveType.ToString() + "&";
                            viceTorqueCurveCount += vh.MomentCurveCount.ToString() + "&";
                            //给力矩曲线留着
                            #region 力矩曲线类型
                            if (vh.MomentCurveType == 1)
                            {
                                try
                                {
                                    foreach (object obj in vh.MomentCurveSet)
                                    {
                                        TowerCraneBasicInformation.MomentCurveSet_Curve mcsc =
                                        Newtonsoft.Json.JsonConvert.DeserializeObject<TowerCraneBasicInformation.MomentCurveSet_Curve>(obj.ToString().Replace("\r\n", ""));
                                        vice_ratioValue += mcsc.Rate.ToString() + "&";
                                        vice_maxLiftWeight += mcsc.MaxWeight.ToString() + "&";
                                        vice_maxLiftWeightRange += mcsc.MaxWeightRange.ToString() + "&";
                                        vice_maxRange += mcsc.MaxRange.ToString() + "&";
                                        vice_maxRangeLiftWeight += mcsc.MaxRangeWeight.ToString() + "&";
                                    }
                                    vice_ratioValue = SQLJoint.RemoveLastChar(vice_ratioValue) + "#";
                                    vice_maxLiftWeight = SQLJoint.RemoveLastChar(vice_maxLiftWeight) + "#";
                                    vice_maxLiftWeightRange = SQLJoint.RemoveLastChar(vice_maxLiftWeightRange) + "#";
                                    vice_maxRange = SQLJoint.RemoveLastChar(vice_maxRange) + "#";
                                    vice_maxRangeLiftWeight = SQLJoint.RemoveLastChar(vice_maxRangeLiftWeight) + "#";
                                }
                                catch (Exception) { }
                            }
                            else if (vh.MomentCurveType == 0)
                            {
                                try
                                {
                                    foreach (object obj in vh.MomentCurveSet)
                                    {
                                        TowerCraneBasicInformation.MomentCurveSet_Icon mcsi =
                                        Newtonsoft.Json.JsonConvert.DeserializeObject<TowerCraneBasicInformation.MomentCurveSet_Icon>(obj.ToString().Replace("\r\n", ""));
                                        vice_ratioValue += mcsi.Rate.ToString() + "&";
                                        vice_curveSetPointNum += mcsi.CurveSetPointCount.ToString() + "&";
                                        foreach (TowerCraneBasicInformation.MomentCurveSet_Icon.MomentCurve_Icon icTemp in mcsi.MomentCurve_IconAry)
                                        {
                                            vice_nPointWeight += icTemp.Weight.ToString() + "&";
                                            vice_nPointWeightRange += icTemp.WeightRange.ToString() + "&";
                                        }
                                        vice_nPointWeight = SQLJoint.RemoveLastChar(vice_nPointWeight) + "#";
                                        vice_nPointWeightRange = SQLJoint.RemoveLastChar(vice_nPointWeightRange) + "#";
                                    }
                                    vice_ratioValue = SQLJoint.RemoveLastChar(vice_ratioValue) + "#";
                                    vice_curveSetPointNum = SQLJoint.RemoveLastChar(vice_curveSetPointNum) + "#";
                                    vice_nPointWeight = SQLJoint.RemoveLastChar(vice_nPointWeight) + "";
                                    vice_nPointWeightRange = SQLJoint.RemoveLastChar(vice_nPointWeightRange) + "";
                                }
                                catch (Exception) { }
                            }
                            #endregion
                        }
                    }
                    catch (Exception) { }
                    SQLJoint.AddField("viceArmLength", SQLJoint.RemoveLastChar(viceArmLength), true, ref text, ref value);
                    SQLJoint.AddField("viceArmAngle", SQLJoint.RemoveLastChar(viceArmAngle), true, ref text, ref value);
                    SQLJoint.AddField("viceTorqueType", SQLJoint.RemoveLastChar(viceTorqueType), true, ref text, ref value);
                    SQLJoint.AddField("viceTorqueCurveCount", SQLJoint.RemoveLastChar(viceTorqueCurveCount), true, ref text, ref value);
                    SQLJoint.AddField("vice_ratioValue", SQLJoint.RemoveLastChar(vice_ratioValue), true, ref text, ref value);
                    SQLJoint.AddField("vice_maxLiftWeight", SQLJoint.RemoveLastChar(vice_maxLiftWeight), true, ref text, ref value);
                    SQLJoint.AddField("vice_maxLiftWeightRange", SQLJoint.RemoveLastChar(vice_maxLiftWeightRange), true, ref text, ref value);
                    SQLJoint.AddField("vice_maxRange", SQLJoint.RemoveLastChar(vice_maxRange), true, ref text, ref value);
                    SQLJoint.AddField("vice_maxRangeLiftWeight", SQLJoint.RemoveLastChar(vice_maxRangeLiftWeight), true, ref text, ref value);
                    SQLJoint.AddField("vice_curveSetPointNum", SQLJoint.RemoveLastChar(vice_curveSetPointNum), true, ref text, ref value);
                    SQLJoint.AddField("vice_nPointWeight", SQLJoint.RemoveLastChar(vice_nPointWeight), true, ref text, ref value);
                    SQLJoint.AddField("vice_nPointWeightRange", SQLJoint.RemoveLastChar(vice_nPointWeightRange), true, ref text, ref value);
                    #endregion
                    SQLJoint.AddField("updateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true, ref text, ref value);
                    text = SQLJoint.RemoveLastChar(text);
                    value = SQLJoint.RemoveLastChar(value);
                    string resultSql = text + value + ")";
                    #endregion

                    int y = DbNet.ExecuteNonQuery(resultSql, null, CommandType.Text);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("塔吊基本信息", o.EquipmentID + ";" + y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {

                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.SaveTowerCraneBasicInformation异常", ex.Message + ex.StackTrace);
                return 0;
            }
        }
        #endregion

        #region 防碰撞设置
        public static int SavePreventCollision(PreventCollision o)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(o.EquipmentID);
                if (DbNet != null)
                {
                    #region 拼接SQL
                    string text = "INSERT INTO t_hitconfig_tower_v2_13 (", value = ") VALUES ( ";
                    SQLJoint.AddField("craneNo", o.EquipmentID, true, ref text, ref value);
                    SQLJoint.AddField("hitNo", o.Number, true, ref text, ref value);
                    SQLJoint.AddField("zigbeeGroupNo", o.GroupNo, true, ref text, ref value);
                    SQLJoint.AddField("zigbeeChannelNo", o.ChannelNo, true, ref text, ref value);
                    //这个要特殊处理一下
                    string cnmTemp = "";
                    try
                    {
                        foreach (byte temp in o.CommunicationNoMapped) { cnmTemp += temp.ToString() + "#"; }
                    }
                    catch (Exception) { }
                    SQLJoint.AddField("zigbeeLocalNo", SQLJoint.RemoveLastChar(cnmTemp), true, ref text, ref value);
                    SQLJoint.AddField("hitWarn", o.PreventCollisionWarning, false, ref text, ref value);
                    SQLJoint.AddField("hitAlarm", o.PreventCollisionAlarm, false, ref text, ref value);
                    SQLJoint.AddField("hitSet", o.PreventCollisionSetType, true, ref text, ref value);

                    PreventCollision.APreventCollisionSet apcs = null;

                    if (o.PreventCollisionSetType == 0)
                    {
                        PreventCollision.PreventCollisionSet_Zero apcsz =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<PreventCollision.PreventCollisionSet_Zero>(o.PreventCollisionSet.ToString().Replace("\r\n", ""));
                        SQLJoint.AddField("coordX", apcsz.X, true, ref text, ref value);
                        SQLJoint.AddField("coordY", apcsz.Y, true, ref text, ref value);
                        apcs = apcsz;
                    }
                    else if (o.PreventCollisionSetType == 1)
                    {
                        PreventCollision.PreventCollisionSet_One apcso =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<PreventCollision.PreventCollisionSet_One>(o.PreventCollisionSet.ToString().Replace("\r\n", ""));
                        SQLJoint.AddField("distance", apcso.Distance, true, ref text, ref value);
                        SQLJoint.AddField("directionRelativeHost", apcso.DirectionTargetOppositeLocal, true, ref text, ref value);
                        SQLJoint.AddField("directionHostRelative", apcso.DirectionLocalOppositeTarget, true, ref text, ref value);
                        apcs = apcso;
                    }
                    SQLJoint.AddField("towerNo", apcs.TowerNo, true, ref text, ref value);
                    SQLJoint.AddField("towerType", apcs.TowerType, true, ref text, ref value);
                    SQLJoint.AddField("armFor", apcs.BoomLength, true, ref text, ref value);
                    SQLJoint.AddField("armBak", apcs.BalanceLength, true, ref text, ref value);
                    SQLJoint.AddField("rootHeight", apcs.TowerBodyHeight, true, ref text, ref value);
                    SQLJoint.AddField("headHeight", apcs.TowerCapHeight, true, ref text, ref value);
                    SQLJoint.AddField("minArmAngle", apcs.MinBoomAngle, true, ref text, ref value);

                    SQLJoint.AddField("updateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true, ref text, ref value);
                    text = SQLJoint.RemoveLastChar(text);
                    value = SQLJoint.RemoveLastChar(value);
                    string resultSql = text + value + ")";
                    #endregion

                    int y = DbNet.ExecuteNonQuery(resultSql, null, CommandType.Text);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("防碰撞设置", o.EquipmentID + ";" + y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {

                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.SavePreventCollision异常", ex.Message + ex.StackTrace);
                return 0;
            }
        }
        #endregion

        #region 区域保护设置设置
        public static int SaveLocalityProtection(LocalityProtection o)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(o.EquipmentID);
                if (DbNet != null)
                {
                    #region 拼接SQL
                    string text = "INSERT INTO t_areaproconfig_tower_v2_13 (", value = ") VALUES ( ";
                    SQLJoint.AddField("craneNo", o.EquipmentID, true, ref text, ref value);
                    SQLJoint.AddField("areaProWarn", o.LocalityProtectionWarning, false, ref text, ref value);
                    SQLJoint.AddField("areaProAlarm", o.LocalityProtectionAlarm, false, ref text, ref value);
                    SQLJoint.AddField("areaProSwitch", o.LocalityProtectionSwitch, true, ref text, ref value);

                    string Range_One = "", Rotation_One = "", Range_Two = "", Rotation_Two = "", Range_Three = "", Rotation_Three = "", Range_Four = "", Rotation_Four = "";
                    try
                    {
                        foreach (LocalityProtection.CLocalityProtectionSet mci in o.LocalityProtectionSet)
                        {
                            Range_One += mci.Range_One.ToString() + "&";
                            Rotation_One += mci.Rotation_One.ToString() + "&";
                            Range_Two += mci.Range_Two.ToString() + "&";
                            Rotation_Two += mci.Rotation_Two.ToString() + "&";
                            Range_Three += mci.Range_Three.ToString() + "&";
                            Rotation_Three += mci.Rotation_Three.ToString() + "&";
                            Range_Four += mci.Range_Four.ToString() + "&";
                            Rotation_Four += mci.Rotation_Four.ToString() + "&";
                        }
                    }
                    catch (Exception) { }
                    SQLJoint.AddField("pointOneRadius", SQLJoint.RemoveLastChar(Range_One), true, ref text, ref value);
                    SQLJoint.AddField("pointOneTurn", SQLJoint.RemoveLastChar(Rotation_One), true, ref text, ref value);
                    SQLJoint.AddField("pointTwoRadius", SQLJoint.RemoveLastChar(Range_Two), true, ref text, ref value);
                    SQLJoint.AddField("pointTwoTurn", SQLJoint.RemoveLastChar(Rotation_Two), true, ref text, ref value);
                    SQLJoint.AddField("pointThreeRadius", SQLJoint.RemoveLastChar(Range_Three), true, ref text, ref value);
                    SQLJoint.AddField("pointThreeTurn", SQLJoint.RemoveLastChar(Rotation_Three), true, ref text, ref value);
                    SQLJoint.AddField("pointFourRadius", SQLJoint.RemoveLastChar(Range_Four), true, ref text, ref value);
                    SQLJoint.AddField("pointFourTurn", SQLJoint.RemoveLastChar(Rotation_Four), true, ref text, ref value);

                    SQLJoint.AddField("updateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true, ref text, ref value);
                    text = SQLJoint.RemoveLastChar(text);
                    value = SQLJoint.RemoveLastChar(value);
                    string resultSql = text + value + ")";
                    #endregion

                    int y = DbNet.ExecuteNonQuery(resultSql, null, CommandType.Text);
                    ToolAPI.XMLOperation.WriteLogXmlNoTail("区域保护", o.EquipmentID + ";" + y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_021303.SaveLocalityProtection异常", ex.Message + ex.StackTrace);
                return 0;
            }
        }
        #endregion

        #region 设备列表的更新
        public static void UpdateDbNetAndSn()
        {
            while (true)
            {
                Thread.Sleep(180000);//3分钟循环一次
                DbNetAndSnInit();
            }
        }
        static void DbNetAndSnInit()
        {
            try
            {
                int flag = 0;
                Dictionary<DbHelperSQL, string> DbNetAndSnTemp = new Dictionary<DbHelperSQL, string>();
                foreach (var item in DbNetAndSn)
                {
                    if (item.Key != null)
                    {
                        IList<DbParameter> paraList = new List<DbParameter>();
                        paraList.Add(item.Key.CreateDbParameter("@ptype", "1"));
                        DataTable o = item.Key.ExecuteDataTable("pro_MosaicStr", paraList, CommandType.StoredProcedure);
                        string value = "";
                        if (o != null && o.Rows.Count > 0)
                            value = o.Rows[0]["classIdsAll"].ToString();
                        DbNetAndSnTemp.Add(item.Key, value);
                    }
                    flag++;
                }
                DbNetAndSn = DbNetAndSnTemp;
            }
            catch { }
        }
        static DbHelperSQL GetDbHelperSQL(string CraneNo)
        {
            try
            {
                foreach (var item in DbNetAndSn)
                {
                    if (!string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(CraneNo))
                    {
                        if (item.Value.Contains(CraneNo))
                            return item.Key;
                    }
                }
                return null;
            }
            catch (Exception ex)
            { return null; }
        }
        #endregion
        /// <summary>
        /// 获取对应属性的值 =和;
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="Attribute"></param>
        /// <returns></returns>
        static private string GetAttributeValue(string sourceString, string Attribute)
        {
            Attribute += "\":";
            return GetValue(sourceString, Attribute, ",");
        }
        /// <summary>
        /// 获取对应属性的值 =和,
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="Attribute"></param>
        /// <returns></returns>
        static private string GetAttributeValueComma(string sourceString, string Attribute)
        {
            Attribute += "\":";
            return GetValue(sourceString, Attribute, "}");
        }
        /// <summary>
        /// 街区一段字符中对应的开始和结束之间的字符
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="start">开始字符串</param>
        /// <param name="end">结束字符串</param>
        /// <returns></returns>
        static private string GetValue(string sourceString, string start, string end)
        {
            Regex rg = new Regex("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(sourceString).Value;
        }
    }

    class StringByDouble
    {
        /// <summary>
        /// 把一个数转化为double类型，并缩小对应的倍数
        /// </summary>
        /// <param name="obj">数</param>
        /// <param name="minification">缩率</param>
        /// <returns>成功返回对应的值，失败返回0</returns>
        public static double ConvertDouble(object obj, int minification)
        {
            double temp = 0d;
            if (double.TryParse(obj.ToString(), out temp))
            {
                return minification != 0 ? (double)(temp / (double)minification) : 0d;
            }
            return 0d;
        }
        /// <summary>
        /// 把一个数转化为double类型，并缩小对应的倍数
        /// </summary>
        /// <param name="obj">数</param>
        /// <param name="minification">缩率</param>
        /// <returns>成功返回对应的值de 字符串形式，失败返回0</returns>
        public static string ConvertDoubleString(object obj, int minification)
        {
            return ConvertDouble(obj, minification).ToString("0.00");
        }
    }
    public class SQLJoint
    {
        /// <summary>
        /// 拼接SQL
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="value">对应值</param>
        /// <param name="isString">是否是字符串</param>
        /// <param name="nameStr">字段名集</param>
        /// <param name="valueStr">值集</param>
        public static void AddField(string name, object value, bool isString, ref string nameStr, ref string valueStr)
        {
            nameStr += name.ToString() + ",";
            //if (isString) valueStr += "'" + value.ToString() + "',";
            //else valueStr += value.ToString() + ",";
            valueStr += "'" + value.ToString() + "',";
        }

        public static string RemoveLastChar(string name)
        {
            try
            {
                if (name.Length <= 0)
                    return name;
                else return name.Substring(0, name.Length - 1);
            }
            catch (Exception) { return ""; }

        }
    }
}
