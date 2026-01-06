#!/bin/bash

echo "=========================================="
echo "Checking Logs for Permissions Issues"
echo "=========================================="

echo ""
echo "1. Checking hc-blazor logs for permissions/auth..."
echo "   (Last 50 lines with permission/auth related)"
docker logs hc-blazor --tail 200 2>&1 | grep -i "permission\|grantedPolicies\|auth\|openid\|oidc" | tail -20

echo ""
echo "2. Checking hc-authserver logs for userinfo/permissions..."
echo "   (Last 50 lines with userinfo/permission related)"
docker logs hc-authserver --tail 200 2>&1 | grep -i "userinfo\|permission\|claim\|token" | tail -20

echo ""
echo "3. Checking hc-blazor logs for errors..."
echo "   (Last 20 error lines)"
docker logs hc-blazor --tail 100 2>&1 | grep -i "error\|exception\|fail" | tail -10

echo ""
echo "4. Checking hc-authserver logs for errors..."
echo "   (Last 20 error lines)"
docker logs hc-authserver --tail 100 2>&1 | grep -i "error\|exception\|fail" | tail -10

echo ""
echo "5. Checking application-configuration API calls..."
docker logs hc-blazor --tail 200 2>&1 | grep -i "application-configuration" | tail -10

echo ""
echo "=========================================="
echo "Recent logs (last 30 lines) from hc-blazor:"
echo "=========================================="
docker logs hc-blazor --tail 30

echo ""
echo "=========================================="
echo "Recent logs (last 30 lines) from hc-authserver:"
echo "=========================================="
docker logs hc-authserver --tail 30

