<template>
  <div id="app">
    <el-menu :default-active="activePath" mode="horizontal" router>
      <!-- 未登录或其他角色 -->
      <template v-if="!role">
        <el-menu-item index="/login">登录</el-menu-item>
        <el-menu-item index="/register">注册</el-menu-item>
      </template>
      <!-- 管理员菜单 -->
      <template v-else-if="role === 'Admin'">
        <el-menu-item index="/admin/users">用户管理</el-menu-item>
        <el-menu-item index="/admin/export">数据导出</el-menu-item>
        <el-menu-item index="/admin/prices-management">价格管理</el-menu-item>
        <el-menu-item index="/admin/publish">政策发布</el-menu-item>
      </template>
      <!-- 农户菜单 -->
      <template v-else-if="role === 'Farmer'">
        <el-menu-item index="/profile">个人信息</el-menu-item>
        <el-menu-item index="/crops">作物管理</el-menu-item>
        <el-menu-item index="/farm-records">农事记录</el-menu-item>
        <el-menu-item index="/prices">价格信息</el-menu-item>
        <el-menu-item index="/policies">政策信息</el-menu-item>
      </template>
      <!-- 退出按钮，已登录时显示 -->
      <el-menu-item v-if="role" index="/login" @click="logout" style="float:right">退出</el-menu-item>
    </el-menu>
    <router-view />
  </div>
</template>

<script setup>
  import { ref, onMounted, watch } from 'vue'
  import { useRouter, useRoute } from 'vue-router'

  const router = useRouter()
  const route = useRoute()
  const role = ref(localStorage.getItem('role') || '')
  const activePath = ref(route.path)

  // 监听路由变化，保持菜单高亮
  watch(
    () => route.path,
    (newPath) => {
      activePath.value = newPath
    }
  )

  // 监听localStorage变化（如登录/退出）
  window.addEventListener('storage', () => {
    role.value = localStorage.getItem('role') || ''
  })

  // 退出登录
  const logout = () => {
    localStorage.removeItem('user')
    localStorage.removeItem('role')
    role.value = ''
    router.push('/login')
  }

  // 登录后刷新role
  onMounted(() => {
    role.value = localStorage.getItem('role') || ''
  })
</script>

<style>
  #app {
    min-height: 100vh;
    background: #f5f7fa;
  }

  .el-menu {
    margin-bottom: 20px;
  }
</style>
