ASM_NAME=AssetBundler.exe
ASM_LOC=./bin/Release
ASSET_PATH=../../assets
TARGET_PATH=../../../Vega/bin

.phony: all img snd build

all: img snd
	cd $(ASM_LOC) && cp *.dat $(TARGET_PATH)/Debug && cp *.dat $(TARGET_PATH)/Release

img:
	cd $(ASM_LOC) && chmod +x $(ASM_NAME) && ./$(ASM_NAME) $(ASSET_PATH)/img VegaImg.dat

snd:
	cd $(ASM_LOC) && chmod +x $(ASM_NAME) && ./$(ASM_NAME) $(ASSET_PATH)/snd VegaSnd.dat

build:
	msbuild
