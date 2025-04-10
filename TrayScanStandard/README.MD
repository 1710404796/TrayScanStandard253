# Linx WCS工程模板

## 1. 项目说明
本项目提供一个通用模板，用于快速创建工程项目的ui工程。

## 2. 项目依赖
项目主要依赖LinxUniverse.NET基线库进行开发
UI依赖于ModernWpfUI库(0.9.6)
设计模式主要采用MVVM(CommunityToolkit.Mvvm 8.2.2), 以及中介者模式(MediatR 12.0.1)进行开发

## 3. 安装方法
克隆项目到本地, 进入目录后输入指令安装
```ps
git clone https://github.com/ssccinng/TrayScanStandard  
## 进入项目目录
cd TrayScanStandard
## 安装本项目模板(--force参数可选，用于强制更新)
dotnet new install ./ --force
```
若显示
```
成功: C:\Users\admin\source\repos\TrayScanStandard 已安装以下模板:
模板名       短名称      语言  标记
-----------  ----------  ----  ----------
LinxWcs模板  linxwcswpf  [C#]  Common/Wpf
```
则已经成功安装


## 4. 使用方法
使用dotnet new指令创建项目（推荐）
```
dotnet new linxwcswpf -o <项目名>
## 例如
dotnet new linxwcswpf -o XCZZJC2024
## 进入项目目录
cd XCZZJC2024

## 创建项目git仓库（可选）

## 克隆LinxUniverse.NET库
git clone https://github.com/ssccinng/LinxUniverse.NET
## 修改LinxUniverse.NET文件夹名字为lxuv后即可正常使用

```

在visual studio或是其他ide中，创建一个自己的项目名分支，**不得使用master分支进行开发！**

## 5. 支持功能

1. webapi功能
	- 
1. Quartz, 定时任务功能
	- 默认提供了一个每天0点10分执行的job(DailyJobs)
1. 身份认证功能，默认密码linx0304
1. 内置拆组盘ui组件
1. 存档管理功能，可以直接修改WCSSave.cs的内容
1. 其他.. 待补充