#!/bin/sh
rootPath=$(pwd)

npm_source_dic_root="$rootPath"/Assets/TapTap/AndroidDependencyResolver

# 多个模块一起发布
# npm_module_name=("Mainland")
npm_module_name=("Account" "Common" "Payment" "Oversea" "Mainland")
npm_module_package_name=("com.xd.sdk.account" "com.xd.sdk.common" "com.xd.sdk.payment" "com.xd.sdk.oversea" "com.xd.sdk.mainland")

# 单个模块发布
#npm_module_name=("Account")

unpublishNPM() {
  echo email=qiankun@xd.com > .npmrc
  echo always-auth=true >> .npmrc
  echo registry=https://nexus.tapsvc.com/repository/npm-registry/ >> .npmrc
  echo //nexus.tapsvc.com/repository/npm-registry/:_authToken=NpmToken.de093789-9551-3238-a766-9d2c694f2600 >> .npmrc
  
  npm unpublish "com.tapsdk.androiddependencyresolver"@"$2"
  
  rm -rf .npmrc
  
  cd "$rootPath" || exit
}

cd "$npm_source_dic_root"

echo "ready to unpublish com.tapsdk.androiddependencyresolver NPM, Path: $npm_source_dic_root"

unpublishNPM $1

echo "npm unpublish com.tapsdk.androiddependencyresolver  finish version: " $1