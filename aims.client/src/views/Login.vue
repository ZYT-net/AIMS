<template>
  <div class="login-container">
    <h2>用户登录</h2>
    <form @submit.prevent="handleLogin">
      <div>
        <input v-model="username"
               placeholder="用户名"
               autocomplete="username" />
      </div>
      <div>
        <input v-model="password"
               type="password"
               placeholder="密码"
               autocomplete="current-password" />
      </div>
      <div>
        <button type="submit" :disabled="loading">登录</button>
      </div>
      <div v-if="errorMsg" class="error">{{ errorMsg }}</div>
    </form>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import { login } from '@/api/user'
  import { setToken } from '@/utils/auth'

  const username = ref('')
  const password = ref('')
  const errorMsg = ref('')
  const loading = ref(false)
  const router = useRouter()

  const handleLogin = async () => {
    errorMsg.value = ''
    loading.value = true
    try {
      const res = await login({
        userName: username.value,
        password: password.value
      })
      setToken(res.data.token)
      localStorage.setItem('user', JSON.stringify(res.data.user))
      localStorage.setItem('role', res.data.user.role)
      // 跳转
      if (res.data.user.role === 'Admin') {
        router.push('/admin/users')
        localStorage.setItem('token', res.data.token)
      } else {
        router.push('/profile')
        localStorage.setItem('token', res.data.token)
      }
    } catch (err) {
      if (err.response && err.response.data && err.response.data.message) {
        errorMsg.value = err.response.data.message
      } else {
        errorMsg.value = '登录失败，请检查用户名和密码'
      }
    } finally {
      loading.value = false
    }
  }
</script>

<style scoped>
  .login-container {
    max-width: 320px;
    margin: 80px auto;
    padding: 32px;
    border: 1px solid #eee;
    border-radius: 8px;
    background: #fff;
  }

  .error {
    color: #d32f2f;
    margin-top: 12px;
  }

  button[disabled] {
    opacity: 0.6;
    cursor: not-allowed;
  }
</style>

