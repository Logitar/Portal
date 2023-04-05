<template>
  <span>
    <icon-button v-if="isDisabled" class="mx-1" :disabled="disabled" icon="unlock" text="user.enable.submit" variant="warning" v-b-modal="id" />
    <icon-button v-else class="mx-1" :disabled="disabled" icon="lock" text="user.disable.submit" variant="warning" v-b-modal="id" />
    <b-modal v-if="!disabled" :id="id" :title="title">
      <p>
        {{ confirm }}
        <br />
        <span class="text-warning">{{ user.fullName ? `${user.fullName} (${user.username})` : user.username }}</span>
      </p>
      <slot />
      <template #modal-footer="{ cancel, ok }">
        <icon-button icon="ban" text="actions.cancel" @click="cancel()" />
        <icon-button v-if="isDisabled" :disabled="loading" icon="unlock" :loading="loading" text="user.enable.submit" variant="warning" @click="enable(ok)" />
        <icon-button v-else :disabled="loading" icon="lock" :loading="loading" text="user.disable.submit" variant="warning" @click="disable(ok)" />
      </template>
    </b-modal>
  </span>
</template>

<script>
import { disableUser, enableUser } from '@/api/users'

export default {
  name: 'ToggleStatus',
  props: {
    disabled: {
      type: Boolean,
      default: false
    },
    user: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      loading: false
    }
  },
  computed: {
    confirm() {
      return this.$i18n.t(this.isDisabled ? 'user.enable.confirm' : 'user.disable.confirm')
    },
    id() {
      return `toggleStatus_${this.user.id}`
    },
    isDisabled() {
      return this.user.isDisabled
    },
    title() {
      return this.$i18n.t(this.isDisabled ? 'user.enable.title' : 'user.disable.title')
    }
  },
  methods: {
    async disable(callback) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await disableUser(this.user.id)
          this.$emit('updated', data)
          this.toast('success', 'user.disable.success')
          callback()
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    },
    async enable(callback) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await enableUser(this.user.id)
          this.$emit('updated', data)
          this.toast('success', 'user.enable.success')
          callback()
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  }
}
</script>
