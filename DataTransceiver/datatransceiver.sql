/*
Navicat MySQL Data Transfer

Source Server         : 192.168.66.38
Source Server Version : 50173
Source Host           : localhost:3306
Source Database       : datatransceiver

Target Server Type    : MYSQL
Target Server Version : 50173
File Encoding         : 65001

Date: 2019-01-03 15:52:07
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for humitureandammonia
-- ----------------------------
DROP TABLE IF EXISTS `humitureandammonia`;
CREATE TABLE `humitureandammonia` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `deviceid` varchar(20) DEFAULT NULL COMMENT '设备编号',
  `datatype` varchar(50) NOT NULL COMMENT '数据类型，判断这个包的类型，比如为心跳，实时数据等',
  `contentjson` text COMMENT '数据内容 json 对象标识，可以直接被序列化的',
  `contenthex` text COMMENT '数据内容原包hex',
  `version` varchar(20) NOT NULL COMMENT '版本号',
  `creattime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '创建时间',
  `usetype` int(10) DEFAULT '0' COMMENT '使用标记',
  `dbtype` int(11) NOT NULL DEFAULT '0' COMMENT '数据库标识 0未进行处理1处理过了',
  `mqtttype` int(11) NOT NULL DEFAULT '0' COMMENT 'mqtttype标识 0未进行处理 1处理过了',
  `forwardtype` int(11) NOT NULL DEFAULT '0' COMMENT '平台转发标识 0未进行处理 1处理过了',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COMMENT='温湿度氨气三合一数据传感器';
