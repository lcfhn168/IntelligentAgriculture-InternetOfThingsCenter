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
using DBStorage.TowerCrane._0E;

namespace DBStorage._0E
{
    public class MysqlTowerCrane_0E
    {
        static DbHelperSQL dbNetdefault = null;
        static DbHelperSQL DbNetHis = null;
        public static Dictionary<string, WorkingCycle> workingCycleList = new Dictionary<string, WorkingCycle>();
        static MysqlTowerCrane_0E()
        {
            try
            {
                string connectionString = ToolAPI.INIOperate.IniReadValue("netSqlGroup", "connectionString", MainStatic.Path);
                string connectionStringHis = ToolAPI.INIOperate.IniReadValue("netSqlGroupHis", "connectionString", MainStatic.Path);

                string[] dbnetAryHis = connectionStringHis.Split('&');
                DbNetHis = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAryHis[0], dbnetAryHis[1], dbnetAryHis[2], dbnetAryHis[3], dbnetAryHis[4]), DbProviderType.MySql);

                string[] dbnetAr = connectionString.Split('&');
                dbNetdefault = new DbHelperSQL(string.Format("Data Source={0};Port={1};Database={2};User={3};Password={4}", dbnetAr[0], dbnetAr[1], dbnetAr[2], dbnetAr[3], dbnetAr[4]), DbProviderType.MySql);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_0E异常", ex.Message);
            }
        }

        public static void TowerCraneTowerCraneDBFrameAnalyse(TowerCraneDBFrame dbf)
        {
            try
            {
                switch (dbf.datatype)
                {
                    case "heartbeat":
                        Heartbeat hb = Newtonsoft.Json.JsonConvert.DeserializeObject<Heartbeat>(dbf.contentjson);
                        Pro_heartbeat(hb); break;
                    case "current":
                        CraneCurrent cu = Newtonsoft.Json.JsonConvert.DeserializeObject<CraneCurrent>(dbf.contentjson);
                        SaveCraneCurrent(cu); break;
                    case "parameterUpload":
                        CraneConfig pu = Newtonsoft.Json.JsonConvert.DeserializeObject<CraneConfig>(dbf.contentjson);
                        SaveCraneConfig(pu); break;
                    case "runtimeEp":
                        CraneRunTime re = Newtonsoft.Json.JsonConvert.DeserializeObject<CraneRunTime>(dbf.contentjson);
                        UpdateRunTime(re); break;
                    default: break;
                }
                MysqlTowerCrane_Local.UpdateTowerCranedbtypeByid(dbf.id);
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_0E.TowerCraneTowerCraneDBFrameAnalyse异常", ex.Message);
            }
        }

        #region 实时数据
        public static int SaveCraneCurrent(CraneCurrent current)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(current.Craneno);
                if (DbNet != null)
                {
                    IList<DbParameter> paraList = new List<DbParameter>();
                    paraList.Add(DbNet.CreateDbParameter("@p_equipmentNo", current.Craneno));
                    paraList.Add(DbNet.CreateDbParameter("@p_data_time", current.Rtime));
                    paraList.Add(DbNet.CreateDbParameter("@p_safe_torque", current.Safetorque));
                    paraList.Add(DbNet.CreateDbParameter("@p_safe_weight", current.SafeWeight));
                    paraList.Add(DbNet.CreateDbParameter("@p_rotation", current.Angle));
                    paraList.Add(DbNet.CreateDbParameter("@p_range", current.Radius));
                    paraList.Add(DbNet.CreateDbParameter("@p_height", current.Height));
                    paraList.Add(DbNet.CreateDbParameter("@p_weight", current.Weight));
                    paraList.Add(DbNet.CreateDbParameter("@p_wind_speed", current.Wind));
                    paraList.Add(DbNet.CreateDbParameter("@p_dip_angle_X", current.AngleX));
                    paraList.Add(DbNet.CreateDbParameter("@p_dip_angle_Y", current.AngleY));
                    paraList.Add(DbNet.CreateDbParameter("@p_rate", current.Times));
                    paraList.Add(DbNet.CreateDbParameter("@p_early_warning", current.WarnType));
                    paraList.Add(DbNet.CreateDbParameter("@p_give_alarm", current.AlarmType));
                    paraList.Add(DbNet.CreateDbParameter("@p_sensor_status", current.SensorStatus));
                    paraList.Add(DbNet.CreateDbParameter("@p_cardNo", current.CardNo));
                    paraList.Add(DbNet.CreateDbParameter("@p_boot_time", current.RTC));
                    paraList.Add(DbNet.CreateDbParameter("@p_relay_status", current.RelayStatus));
                    paraList.Add(DbNet.CreateDbParameter("@p_Longitude", current.Longitude));
                    paraList.Add(DbNet.CreateDbParameter("@p_Latitude", current.Latitude));

                    int y = DbNet.ExecuteNonQuery("pro_craneCurrent", paraList, CommandType.StoredProcedure);
                    if (y > 0)
                    { 
                        int y1 = DbNetHis.ExecuteNonQuery("pro_exepro_crane", paraList, CommandType.StoredProcedure);
                    }
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail("实时数据C", current.Craneno + ";" + y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_0E.SaveCraneCurrent异常", ex.Message);
                return 0;
            }
        }

        #region 工作循环判定
        /// <summary>
        /// 塔吊更新工作循环
        /// </summary>
        /// 规则：吊重开始--下一次吊重开始（或无心跳），设备重新开机（心跳/实时数据从无到有）重量低于0.2吨的数据丢弃，从第一次符合工作循环起始条件的数据开始统计。
        /// 起始条件：大于0.2t&&（高度或幅度或回转有变化）。
        /// 时间：大于等于1分钟，如果小于1分钟，则属于上一个工作循环。
        public static string updateWorkingCycle(CraneCurrent current)
        {
            try
            {
                DateTime start = DateTime.Now;

                WorkingCycle workingCycle = null;

                if (workingCycleList.Keys.Contains(current.Craneno))
                    workingCycle = workingCycleList[current.Craneno];
                else
                {
                    workingCycle = new WorkingCycle();
                    workingCycleList.Add(current.Craneno, workingCycle);
                }
                #region 公共头处理
                //CraneCurrent current = dataTemp.Current;
                bool isWhight = double.Parse(current.Weight) > workingCycle.WeightCriticalValue;//重量
                bool isChangeHeight = false; //高度
                bool isChangeRange = false;//幅度
                bool isChangeRotationAngle = false;
                try
                {
                    isChangeHeight = Math.Abs(double.Parse(current.Height) - double.Parse(workingCycle.lastHeight)) > workingCycle.HeightRangeCriticalValue; //高度
                    isChangeRange = Math.Abs(double.Parse(current.Radius) - double.Parse(workingCycle.lastRange)) > workingCycle.HeightRangeCriticalValue;//幅度
                    isChangeRotationAngle = Math.Abs(double.Parse(current.Angle) - double.Parse(workingCycle.lastRotationAngle)) > workingCycle.RotationCriticalValue;
                }
                catch (Exception)
                { }
                #endregion
              
                #region 工作循环逻辑处理
                if (!workingCycle.WCID.Equals(""))
                {
                    //更新lastTime
                    GetWorkingCycle(current.Craneno, "updateFinallyTime", "", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                if (workingCycle.isNewWorkingCycle)//新的工作循环
                {
                    if (isWhight && (isChangeHeight || isChangeRange || isChangeRotationAngle))//符合新工作循环的条件
                    {
                        //获取数据库中的数据
                        DataTable dt = GetWorkingCycle(current.Craneno, "get", "", "", null);
                        DateTime finallyTemp = Convert.ToDateTime(null);//做后一次刷新的时间
                        DateTime startTemp = Convert.ToDateTime(null); //工作循环开始的使劲按
                        string isWc = "";
                        string finallyWcid = "";
                        if (dt != null && dt.Rows.Count >= 1)
                        {
                            finallyWcid = dt.Rows[0]["finallyWCID"].ToString();
                            DateTime.TryParse(dt.Rows[0]["finallyTime"].ToString(), out finallyTemp);
                            DateTime.TryParse(dt.Rows[0]["startTime"].ToString(), out startTemp);
                            isWc = dt.Rows[0]["isEnd"].ToString();
                        }

                        if (workingCycle.isFirstCurrent)//第一次连接后
                        {

                            if (finallyTemp.ToString("yyyyMMdd HHmmss").Equals("00010101 000000") || (!finallyTemp.ToString("yyyyMMdd HHmmss").Equals("00010101 000000") && (DateTime.Now - finallyTemp).TotalMinutes > 5))
                            {
                                if (isWc.Equals("true"))
                                {
                                    workingCycle.WCID = (Int64.Parse(finallyWcid) + 1L).ToString();
                                    workingCycle.StartTime = DateTime.Now;
                                    //写入数据库
                                    //更新序列号
                                    GetWorkingCycle(current.Craneno, "updateWCID", workingCycle.WCID, "", null);
                                    //更新开始时间和false
                                    GetWorkingCycle(current.Craneno, "updateStartTime", "", "", workingCycle.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    //更新实时数据和工作循环的表
                                    GetWorkingCycle(current.Craneno, "updateCurrentWcid", finallyWcid, (Int64.Parse(finallyWcid) - 1L).ToString(), null);
                                    //序列号不变
                                    workingCycle.WCID = finallyWcid;
                                    //更新开始时间和false
                                    GetWorkingCycle(current.Craneno, "updateStartTime", "", "", workingCycle.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            }
                            else
                            {
                                //序列号不变
                                workingCycle.WCID = finallyWcid;
                                //更新lastTime
                                GetWorkingCycle(current.Craneno, "updateFinallyTime", "", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            //第一次连接后的未成功当作一次工作循环的标志给标记上
                            workingCycle.isFirstWC = 1;
                            workingCycle.isFirstCurrent = false;
                            #region 说明
                            //if 大于5分钟
                            //if iswc=true   
                            /**
                             * id++
                             * ST=LT=Now();
                             * iswc=false
                             * **/
                            //else iswc=false
                            /**
                             * 把原来带有id的实时数据的id-1,且把原来工作t_onceweighthistory中进行更新
                            * id = id
                            * ST=LT=Now();
                            * iswc=false
                            * **/
                            //else 小于5分钟
                            /**
                             * id = id
                          *更新lastTime 的处理
                           * **/
                            //把第一次连接后的未成功的标志给标记上
                            #endregion
                        }
                        else
                        {

                            if (isWc.Equals("true"))
                            {
                                workingCycle.WCID = (Int64.Parse(finallyWcid) + 1L).ToString();
                                workingCycle.StartTime = DateTime.Now;
                                //写入数据库
                                //更新序列号
                                GetWorkingCycle(current.Craneno, "updateWCID", workingCycle.WCID, "", null);
                                //更新开始时间和false
                                GetWorkingCycle(current.Craneno, "updateStartTime", "", "", workingCycle.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                                workingCycle.isFirstWC = 0;
                            }
                            else
                            {
                                if (workingCycle.isFirstWC == 1)
                                {
                                    GetWorkingCycle(current.Craneno, "updateCurrentWcid", finallyWcid, "", null);
                                }
                                else
                                {
                                    GetWorkingCycle(current.Craneno, "updateCurrentWcid", finallyWcid, (Int64.Parse(finallyWcid) - 1L).ToString(), null);
                                }
                                //序列号不变
                                workingCycle.WCID = finallyWcid;
                                //更新开始时间和false
                                GetWorkingCycle(current.Craneno, "updateStartTime", "", "", workingCycle.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            #region 说明
                            //if   iswc=true
                            /*
                             * 数据库处理
                             * id++
                             * ST=LT=Now();
                             * iswc=false
                             * 第一次连接后的未成功的标志给标记成立撤销
                             * /
                        //else iswc=false
                             * if 把第一次连接后的未成功的标志给标记成立
                                把原来带有id的实时数据的改为"",且把原来工作t_onceweighthistory中进行更新
                             * else
                             * 把原来带有id的实时数据的id-1,且把原来工作t_onceweighthistory中进行更新
                             *id = id
                             * ST=LT=Now();
                             * iswc=false
                             */
                            #endregion
                        }
                        workingCycle.isNewWorkingCycle = false;
                    }
                }
                else//存在的工作循环
                {
                    if (!isWhight)
                    {
                        //把isNewWorkingCycle设置成true
                        workingCycle.isNewWorkingCycle = true;
                    }
                }
                #endregion

                #region 公共尾处理
                //把这个工作循环序列号赋值给实时数据
                workingCycle.lastHeight = current.Height;
                workingCycle.lastRange = current.Radius;
                workingCycle.lastRotationAngle = current.Angle;
                //ToolAPI.XMLOperation.WriteLogXmlNoTail("时间", (DateTime.Now-start).TotalMilliseconds.ToString());
                return workingCycle.WCID;
                #endregion
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_0E.updateWorkingCycle异常",current.Craneno+ ex.Message +ex.StackTrace );
                return "0";
            }
        }

        public static DataTable GetWorkingCycle(string craneNo, string command, string wcidone, string wcidTwo, string times)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(craneNo);
                if (DbNet != null)
                {
                    IList<DbParameter> paraList = new List<DbParameter>();
                    paraList.Add(DbNet.CreateDbParameter("@zcraneNo", craneNo));
                    paraList.Add(DbNet.CreateDbParameter("@command", command));
                    paraList.Add(DbNet.CreateDbParameter("@wcidone", wcidone));
                    paraList.Add(DbNet.CreateDbParameter("@wcidTwo", wcidTwo));
                    paraList.Add(DbNet.CreateDbParameter("@times", times));
                    DataTable dt = DbNet.ExecuteDataTable("pro_UpdateWorkCircle", paraList, CommandType.StoredProcedure);
                    if (dt == null)
                    {
                        return null;
                    }
                    return dt;
                }
                return null;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_0E.GetWorkingCycle异常", ex.Message);
                return null;
            }
        }
        #endregion
        #endregion

        #region 心跳
        public static int Pro_heartbeat(Heartbeat o)
        {
            try
            {
                DbHelperSQL DbNet = GetDbHelperSQL(o.SN);
                if (DbNet != null)
                {
                    IList<DbParameter> paraList = new List<DbParameter>();
                    paraList.Add(DbNet.CreateDbParameter("@craneNo", o.SN));
                    paraList.Add(DbNet.CreateDbParameter("@onlineTimes", o.OnlineTime));
                    int y = DbNet.ExecuteNonQuery("pro_heartbeat", paraList, CommandType.StoredProcedure);
                    //ToolAPI.XMLOperation.WriteLogXmlNoTail("心跳C", o.SN +";"+ y.ToString());
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_0E.Pro_heartbeat异常", ex.Message);
                return 0;
            }

        }
        #endregion

        #region 参数上传
        public static int SaveCraneConfig(CraneConfig model)
        {
            try
            {
                #region
                //DbHelperSQL DbNet = GetDbHelperSQL(model.craneNo);
                //if (DbNet != null)
                //{
                //    IList<DbParameter> paraList = new List<DbParameter>();
                //    paraList.Add(DbNet.CreateDbParameter("@craneNo", model.craneNo));
                //    paraList.Add(DbNet.CreateDbParameter("@softVersion", model.softVersion));
                //    paraList.Add(DbNet.CreateDbParameter("@ratio", model.ratio));
                //    paraList.Add(DbNet.CreateDbParameter("@minHighAD", model.minHighAD));
                //    paraList.Add(DbNet.CreateDbParameter("@maxHighAD", model.maxHighAD));
                //    paraList.Add(DbNet.CreateDbParameter("@standardScale", model.standardScale));
                //    paraList.Add(DbNet.CreateDbParameter("@minAmplitude", model.minAmplitude));
                //    paraList.Add(DbNet.CreateDbParameter("@minAmplitudeAD", model.minAmplitudeAD));
                //    paraList.Add(DbNet.CreateDbParameter("@maxAmplitude", model.maxAmplitude));
                //    paraList.Add(DbNet.CreateDbParameter("@maxAmplitudeAD", model.maxAmplitudeAD));
                //    paraList.Add(DbNet.CreateDbParameter("@emptyhookAD", model.emptyhookAD));
                //    paraList.Add(DbNet.CreateDbParameter("@loadWeightAD", model.loadWeightAD));
                //    paraList.Add(DbNet.CreateDbParameter("@farmarWeight", model.farmarWeight));
                //    paraList.Add(DbNet.CreateDbParameter("@rotaryType", model.rotaryType));
                //    paraList.Add(DbNet.CreateDbParameter("@absTurnDirection", model.absTurnDirection));
                //    paraList.Add(DbNet.CreateDbParameter("@absTurnValue", model.absTurnValue));
                //    paraList.Add(DbNet.CreateDbParameter("@absTurnPointValue", model.absTurnPointValue));
                //    paraList.Add(DbNet.CreateDbParameter("@potLeftLimitAD", model.potLeftLimitAD));
                //    paraList.Add(DbNet.CreateDbParameter("@potRightLimitAD", model.potRightLimitAD));
                //    paraList.Add(DbNet.CreateDbParameter("@potLimitAngle", model.potLimitAngle));
                //    paraList.Add(DbNet.CreateDbParameter("@liftWeight4Ratio", model.liftWeight4Ratio));
                //    paraList.Add(DbNet.CreateDbParameter("@liftWeightRange4R", model.liftWeightRange4R));
                //    paraList.Add(DbNet.CreateDbParameter("@maxRange4Ratio", model.maxRange4Ratio));
                //    paraList.Add(DbNet.CreateDbParameter("@maxRangeWeight4R", model.maxRangeWeight4R));
                //    paraList.Add(DbNet.CreateDbParameter("@liftWeight2Ratio", model.liftWeight2Ratio));
                //    paraList.Add(DbNet.CreateDbParameter("@liftWeightRange2R", model.liftWeightRange2R));
                //    paraList.Add(DbNet.CreateDbParameter("@maxRange2Ratio", model.maxRange2Ratio));
                //    paraList.Add(DbNet.CreateDbParameter("@maxRangeWeight2R", model.maxRangeWeight2R));
                //    paraList.Add(DbNet.CreateDbParameter("@zigbeeLocalNo", model.zigbeeLocalNo));
                //    paraList.Add(DbNet.CreateDbParameter("@zigbeeChannelNo", model.zigbeeChannelNo));
                //    paraList.Add(DbNet.CreateDbParameter("@zigbeeGroupNo", model.zigbeeGroupNo));
                //    paraList.Add(DbNet.CreateDbParameter("@antiCollisionX", model.antiCollisionX));
                //    paraList.Add(DbNet.CreateDbParameter("@antiCollisionY", model.antiCollisionY));
                //    paraList.Add(DbNet.CreateDbParameter("@liftWeightArmLenght", model.liftWeightArmLenght));
                //    paraList.Add(DbNet.CreateDbParameter("@balanceArmLenght", model.balanceArmLenght));
                //    paraList.Add(DbNet.CreateDbParameter("@towerHeight", model.towerHeight));
                //    paraList.Add(DbNet.CreateDbParameter("@towerAtHeight", model.towerAtHeight));
                //    paraList.Add(DbNet.CreateDbParameter("@ampReductionValue", model.ampReductionValue));
                //    paraList.Add(DbNet.CreateDbParameter("@ampRestrictValue", model.ampRestrictValue));
                //    paraList.Add(DbNet.CreateDbParameter("@highReductionValue", model.highReductionValue));
                //    paraList.Add(DbNet.CreateDbParameter("@highRestrictValue", model.highRestrictValue));
                //    paraList.Add(DbNet.CreateDbParameter("@turnReducionValue", model.turnReducionValue));
                //    paraList.Add(DbNet.CreateDbParameter("@turnRestrictValue", model.turnRestrictValue));
                //    paraList.Add(DbNet.CreateDbParameter("@areaReductionValue", model.areaReductionValue));
                //    paraList.Add(DbNet.CreateDbParameter("@areaRestrictValue", model.areaRestrictValue));
                //    paraList.Add(DbNet.CreateDbParameter("@acReductionValue", model.acReductionValue));
                //    paraList.Add(DbNet.CreateDbParameter("@acRestrictValue", model.acRestrictValue));
                //    paraList.Add(DbNet.CreateDbParameter("@throwOverTorque", model.throwOverTorque));
                //    paraList.Add(DbNet.CreateDbParameter("@cutTorque", model.cutTorque));
                //    paraList.Add(DbNet.CreateDbParameter("@throwOverWeight", model.throwOverWeight));
                //    paraList.Add(DbNet.CreateDbParameter("@cutWeight", model.cutWeight));
                //    int y = DbNet.ExecuteNonQuery("pro_craneConfig", paraList, CommandType.StoredProcedure);
                //    return y;
                //}
                #endregion
                DbHelperSQL DbNet = GetDbHelperSQL(model.craneNo);
                if (DbNet != null)
                {
                    IList<DbParameter> paraList = new List<DbParameter>();
                    if (string.IsNullOrEmpty(model.RTC))
                        model.RTC = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    paraList.Add(DbNet.CreateDbParameter("@p_equipmentNo", model.craneNo));
                    paraList.Add(DbNet.CreateDbParameter("@p_data_time", model.RTC));      //新加参数
                    paraList.Add(DbNet.CreateDbParameter("@p_soft_version", model.softVersion));
                    paraList.Add(DbNet.CreateDbParameter("@p_rate", model.ratio));
                    paraList.Add(DbNet.CreateDbParameter("@p_min_height_sensor_value", model.minHighAD));
                    paraList.Add(DbNet.CreateDbParameter("@p_max_height_sensor_value", model.maxHighAD));
                    paraList.Add(DbNet.CreateDbParameter("@p_standard_length", model.standardScale));
                    paraList.Add(DbNet.CreateDbParameter("@p_min_range", model.minAmplitude));
                    paraList.Add(DbNet.CreateDbParameter("@p_min_range_sensor_value", model.minAmplitudeAD));
                    paraList.Add(DbNet.CreateDbParameter("@p_max_range", model.maxAmplitude));
                    paraList.Add(DbNet.CreateDbParameter("@p_max_range_sensor_value", model.maxAmplitudeAD));
                    paraList.Add(DbNet.CreateDbParameter("@p_empty_hook_AD_value", model.emptyhookAD));
                    paraList.Add(DbNet.CreateDbParameter("@p_weight_sensor_value", model.loadWeightAD));
                    paraList.Add(DbNet.CreateDbParameter("@p_weight_weight", model.farmarWeight));
                    paraList.Add(DbNet.CreateDbParameter("@p_rotation_type", model.rotaryType));
                    paraList.Add(DbNet.CreateDbParameter("@p_absolute_rotation_direction", model.absTurnDirection));
                    paraList.Add(DbNet.CreateDbParameter("@p_absolute_rotation", model.absTurnValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_absolute_rotationpoint_confirm", model.absTurnPointValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_potentiometer_rotation_leftlimit_sensor_value", model.potLeftLimitAD));
                    paraList.Add(DbNet.CreateDbParameter("@p_potentiometer_rotation_rightlimit_sensor_value", model.potRightLimitAD));
                    paraList.Add(DbNet.CreateDbParameter("@p_potentiometer_rotation_limit_anglesum", model.potLimitAngle));
                    paraList.Add(DbNet.CreateDbParameter("@p_four_rate_max_weight", model.liftWeight4Ratio));
                    paraList.Add(DbNet.CreateDbParameter("@p_four_rate_max_weight_range", model.liftWeightRange4R));
                    paraList.Add(DbNet.CreateDbParameter("@p_four_rate_max_range", model.maxRange4Ratio));
                    paraList.Add(DbNet.CreateDbParameter("@p_four_rate_max_range_weight", model.maxRangeWeight4R));
                    paraList.Add(DbNet.CreateDbParameter("@p_two_rate_max_weight", model.liftWeight2Ratio));
                    paraList.Add(DbNet.CreateDbParameter("@p_two_rate_max_weight_range", model.liftWeightRange2R));
                    paraList.Add(DbNet.CreateDbParameter("@p_two_rate_max_range", model.maxRange2Ratio));
                    paraList.Add(DbNet.CreateDbParameter("@p_two_rate_max_range_weight", model.maxRangeWeight2R));
                    paraList.Add(DbNet.CreateDbParameter("@p_zigbee_local_No", model.zigbeeLocalNo));
                    paraList.Add(DbNet.CreateDbParameter("@p_zigbee_channel_No", model.zigbeeChannelNo));
                    paraList.Add(DbNet.CreateDbParameter("@p_zigbee_group_No", model.zigbeeGroupNo));
                    paraList.Add(DbNet.CreateDbParameter("@p_anti_collision_local_X", model.antiCollisionX));
                    paraList.Add(DbNet.CreateDbParameter("@p_anti_collision_local_Y", model.antiCollisionY));
                    paraList.Add(DbNet.CreateDbParameter("@p_lifting_arm_length", model.liftWeightArmLenght));
                    paraList.Add(DbNet.CreateDbParameter("@p_balance_arm_length", model.balanceArmLenght));
                    paraList.Add(DbNet.CreateDbParameter("@p_tower_body_height", model.towerHeight));
                    paraList.Add(DbNet.CreateDbParameter("@p_tower_hat_height", model.towerAtHeight));
                    paraList.Add(DbNet.CreateDbParameter("@p_range_decelerate", model.ampReductionValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_range_speedlimit", model.ampRestrictValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_height_decelerate", model.highReductionValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_height_speedlimit", model.highRestrictValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_rotation_decelerate", model.turnReducionValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_rotation_speedlimit", model.turnRestrictValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_locality_protection_decelerate", model.areaReductionValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_locality_protection_speedlimit", model.areaRestrictValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_anti_collision_decelerate", model.acReductionValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_anti_collision_speedlimit", model.acRestrictValue));
                    paraList.Add(DbNet.CreateDbParameter("@p_speed_exchange_torque", model.throwOverTorque));
                    paraList.Add(DbNet.CreateDbParameter("@p_cutting_torque", model.cutTorque));
                    paraList.Add(DbNet.CreateDbParameter("@p_speed_exchange_weight", model.throwOverWeight));
                    paraList.Add(DbNet.CreateDbParameter("@p_cutting_weight", model.cutWeight));
                    int y = DbNet.ExecuteNonQuery("pro_craneParameter", paraList, CommandType.StoredProcedure);
                    return y;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ToolAPI.XMLOperation.WriteLogXmlNoTail("MysqlTowerCrane_0E.SaveCraneConfig异常", ex.Message);
                return 0;
            }
        }
        #endregion

        #region 运行时间
        public static void UpdateRunTime(CraneRunTime o)
        {

            //string sql = String.Format(" update  t_cranecurrent_tower set runTime='{0}',sumRunTime='{1}' where craneNo='{2}'", o.Run_second, o.Run_second_sum, o.CraneNo);
            //MainStatic.CSMS40_DB.ExecuteNonQuery(sql, null, CommandType.Text);
            return;
        }
        #endregion


        #region 设备列表的更新

        static DbHelperSQL GetDbHelperSQL(string CraneNo)
        {
            try
            {
                return dbNetdefault;//没有找到，就做成默认数据库连接呗
            }
            catch (Exception ex)
            { return null; }
        }
        #endregion
    }
}
