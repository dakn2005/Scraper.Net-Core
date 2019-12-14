-- MySQL dump 10.17  Distrib 10.3.12-MariaDB, for osx10.13 (x86_64)
--
-- Host: localhost    Database: stcdb
-- ------------------------------------------------------
-- Server version	10.3.12-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `sites`
--

DROP TABLE IF EXISTS `sites`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sites` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `sitename` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `sitehref` text COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sites`
--

LOCK TABLES `sites` WRITE;
/*!40000 ALTER TABLE `sites` DISABLE KEYS */;
INSERT INTO `sites` VALUES (1,'localhost:17310','http://localhost:17310/');
/*!40000 ALTER TABLE `sites` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wordstats`
--

DROP TABLE IF EXISTS `wordstats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `wordstats` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `site_id` int(11) NOT NULL,
  `word` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `cnt` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=101 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wordstats`
--

LOCK TABLES `wordstats` WRITE;
/*!40000 ALTER TABLE `wordstats` DISABLE KEYS */;
INSERT INTO `wordstats` VALUES (1,1,'wiki',20),(2,1,'land',7),(3,1,'ting',7),(4,1,'wikipedia',6),(5,1,'page',6),(6,1,'the',6),(7,1,'ida',5),(8,1,'led',5),(9,1,'use',5),(10,1,'old',4),(11,1,'san',4),(12,1,'new',4),(13,1,'win',4),(14,1,'state',4),(15,1,'media',4),(16,1,'norsk',4),(17,1,'hrvatski',4),(18,1,'view',4),(19,1,'edit',3),(20,1,'article',3),(21,1,'elect',3),(22,1,'did',3),(23,1,'know',3),(24,1,'change',3),(25,1,'work',3),(26,1,'news',3),(27,1,'recent',3),(28,1,'picture',3),(29,1,'portal',3),(30,1,'wikidata',3),(31,1,'species',3),(32,1,'bahasa',3),(33,1,'free',2),(34,1,'main',2),(35,1,'english',2),(36,1,'featured',2),(37,1,'york',2),(38,1,'nuns',2),(39,1,'house',2),(40,1,'prioress',2),(41,1,'south',2),(42,1,'business',2),(43,1,'sued',2),(44,1,'rose',2),(45,1,'royal',2),(46,1,'radar',2),(47,1,'navy',2),(48,1,'used',2),(49,1,'developer',2),(50,1,'start',2),(51,1,'major',2),(52,1,'events',2),(53,1,'doping',2),(54,1,'day',2),(55,1,'john',2),(56,1,'flood',2),(57,1,'government',2),(58,1,'list',2),(59,1,'bald',2),(60,1,'eagle',2),(61,1,'national',2),(62,1,'here',2),(63,1,'rest',2),(64,1,'community',2),(65,1,'help',2),(66,1,'communication',2),(67,1,'commons',2),(68,1,'mediawiki',2),(69,1,'metawiki',2),(70,1,'project',2),(71,1,'wikibooks',2),(72,1,'base',2),(73,1,'wikinews',2),(74,1,'wikiquote',2),(75,1,'wikisource',2),(76,1,'wikispecies',2),(77,1,'wikiversity',2),(78,1,'wikivoyage',2),(79,1,'wiktionary',2),(80,1,'deutsch',2),(81,1,'espaol',2),(82,1,'franais',2),(83,1,'italiano',2),(84,1,'nederlands',2),(85,1,'polski',2),(86,1,'portugus',2),(87,1,'svenska',2),(88,1,'vit',2),(89,1,'indonesia',2),(90,1,'melayu',2),(91,1,'catal',2),(92,1,'etina',2),(93,1,'dansk',2),(94,1,'esperanto',2),(95,1,'euskara',2),(96,1,'magyar',2),(97,1,'romn',2),(98,1,'srpski',2),(99,1,'srpskohrvatski',2),(100,1,'suomi',2);
/*!40000 ALTER TABLE `wordstats` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-12-14 19:06:17
